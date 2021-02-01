using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Erabikata.Backend.DataProviders;
using Erabikata.Backend.Models.Actions;
using Erabikata.Backend.Models.Database;
using Erabikata.Models.Input.V2;
using Grpc.Core.Utils;
using MongoDB.Bson;
using MongoDB.Driver;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Erabikata.Backend.CollectionManagers
{
    public class DialogCollectionManager : ICollectionManager
    {
        private readonly IReadOnlyDictionary<AnalyzerMode, IMongoCollection<Dialog>>
            _mongoCollections;

        private readonly AnalyzerService.AnalyzerServiceClient _analyzerServiceClient;
        private readonly SeedDataProvider _seedDataProvider;

        public DialogCollectionManager(
            IMongoDatabase database,
            AnalyzerService.AnalyzerServiceClient analyzerServiceClient,
            SeedDataProvider seedDataProvider)
        {
            _analyzerServiceClient = analyzerServiceClient;
            _seedDataProvider = seedDataProvider;
            _mongoCollections =
                new[] {AnalyzerMode.SudachiA, AnalyzerMode.SudachiB, AnalyzerMode.SudachiC}
                    .ToDictionary(
                        mode => mode,
                        mode => database.GetCollection<Dialog>(nameof(Dialog) + mode)
                    );
        }

        public async Task OnActivityExecuting(Activity activity)
        {
            switch (activity)
            {
                case BeginIngestion:
                    await Task.WhenAll(
                        _mongoCollections.Values.Select(
                            collection => collection.DeleteManyAsync(FilterDefinition<Dialog>.Empty)
                        )
                    );
                    await IngestDialog();
                    break;
            }
        }

        private async Task IngestDialog()
        {
            var showFiles = _seedDataProvider.GetShowMetadataFiles();
            var tasks = await Task.WhenAll(
                showFiles.Select(
                    async metadataFilePath =>
                    {
                        var metadata = await DeserializeFile<ShowInfo>(metadataFilePath);
                        return metadata.Episodes.Select(
                            (episode, index) => new
                            {
                                path = SeedDataProvider.GetDataPath(
                                    "input",
                                    metadataFilePath,
                                    index
                                ),
                                id = int.Parse(episode[0].Key.Split('/').Last())
                            }
                        );
                    }
                )
            );
            foreach (var (analyzerMode, mongoCollection) in _mongoCollections)
            {
                var jobs = tasks.SelectMany(a => a)
                    .Select(
                        async job =>
                        {
                            var input = await DeserializeFile<InputSentence[]>(job.path);
                            var client = _analyzerServiceClient.AnalyzeDialogBulk();
#pragma warning disable 4014
                            client.RequestStream.WriteAllAsync(
                                    input.Select(
                                        sentence =>
                                        {
                                            var request = new AnalyzeDialogRequest
                                            {
                                                Style = sentence.Style,
                                                Time = sentence.Time,
                                                Mode = analyzerMode
                                            };
                                            request.Lines.Add(sentence.Text);
                                            return request;
                                        }
                                    )
                                )
                                .ConfigureAwait(false);
#pragma warning restore 4014
                            var responses = await client.ResponseStream.ToListAsync();
                            await mongoCollection.InsertManyAsync(
                                responses.Select(
                                    response => new Dialog(ObjectId.Empty, job.id, response.Time)
                                    {
                                        Lines = response.Lines.Select(
                                                line => new Dialog.Line(
                                                    line.Words.Select(
                                                            word => new Dialog.Word(
                                                                word.BaseForm,
                                                                word.DictionaryForm,
                                                                word.Original,
                                                                word.Reading
                                                            )
                                                            {
                                                                PartOfSpeech =
                                                                    word.PartOfSpeech.ToArray()
                                                            }
                                                        )
                                                        .ToArray()
                                                )
                                            )
                                            .ToArray()
                                    }
                                )
                            );
                        }
                    );
                await Task.WhenAll(jobs);
            }
        }

        public async Task<IReadOnlyList<Dialog>> GetEpisodeDialog(
            int episodeId,
            AnalyzerMode analyzerMode)
        {
            return await _mongoCollections[analyzerMode]
                .Find(dialog => dialog.EpisodeId == episodeId)
                .SortBy(dialog => dialog.Time)
                .ToListAsync();
        }

        private static async Task<T> DeserializeFile<T>(string path)
        {
            await using var file = File.OpenRead(path);
            var results = await JsonSerializer.DeserializeAsync<T>(
                file,
                new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase}
            );
            return results!;
        }

        private const int TimeDelta = 10;

        public Task<List<Dialog>> GetNearestDialog(
            int episodeId,
            double time,
            int count,
            AnalyzerMode analyzerMode) =>
            _mongoCollections[analyzerMode]
                .Find(
                    dialog => dialog.EpisodeId == episodeId && dialog.Time > time - TimeDelta &&
                              dialog.Time < time + TimeDelta
                )
                .SortBy(dialog => dialog.Time)
                .ToListAsync();

        public Task<List<Dialog>> GetMatches(
            string baseForm,
            AnalyzerMode analyzerMode,
            int skip,
            int take) =>
            Find(baseForm, analyzerMode).Skip(skip).Limit(take).ToListAsync();

        public Task<long> CountMatches(string baseForm, AnalyzerMode mode) =>
            Find(baseForm, mode).CountDocumentsAsync();

        private IFindFluent<Dialog, Dialog> Find(string baseForm, AnalyzerMode analyzerMode) =>
            _mongoCollections[analyzerMode]
                .Find(
                    dialog => dialog.Lines.Any(
                        line => line.Words.Any(word => word.BaseForm == baseForm)
                    )
                );
    }
}