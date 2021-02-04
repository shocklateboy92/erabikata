using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Erabikata.Backend.DataProviders;
using Erabikata.Backend.Models.Actions;
using Erabikata.Backend.Models.Database;
using Erabikata.Models.Input.V2;
using Grpc.Core.Utils;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

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
            var showEpisode = await _seedDataProvider.GetShowMetadataFiles("input", "json");
            foreach (var (analyzerMode, mongoCollection) in _mongoCollections)
            {
                var jobs = showEpisode.SelectMany(a => a)
                    .Select(
                        async job =>
                        {
                            var input =
                                await SeedDataProvider.DeserializeFile<InputSentence[]>(job.Path);
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
                                    response => new Dialog(ObjectId.Empty, job.Id, response.Time)
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
            int skip = 0,
            int take = int.MaxValue) =>
            Find(baseForm, analyzerMode).Skip(skip).Limit(take).ToListAsync();

        public Task<List<Dialog>>
            GetFuzzyMatches(string baseOrDictionaryForm, AnalyzerMode analyzerMode) =>
            _mongoCollections[analyzerMode]
                .Find(
                    dialog => dialog.Lines.Any(
                        line => line.Words.Any(
                            word => word.BaseForm.Contains(baseOrDictionaryForm) ||
                                    word.DictionaryForm.Contains(baseOrDictionaryForm)
                        )
                    )
                )
                .ToListAsync();

        public Task<long> CountMatches(string baseForm, AnalyzerMode mode) =>
            Find(baseForm, mode).CountDocumentsAsync();

        public async Task<List<WordRank>> GetSortedWordCounts(AnalyzerMode mode)
        {
            var results = await _mongoCollections[mode]
                .AggregateAsync(
                    (PipelineDefinition<Dialog, WordRank>) new BsonDocument[]
                    {
                        new()
                        {
                            {
                                "$project",
                                new BsonDocument("BaseForm", "$Lines.Words.BaseForm")
                            }
                        },
                        new() {{"$unwind", "$BaseForm"}},
                        new() {{"$unwind", "$BaseForm"}}, new() {{"$sortByCount", "$BaseForm"}}
                    }
                );
            return await results.ToListAsync();
        }

        public record WordRank(
            [property: BsonId] string BaseForm,
            [property: BsonElement("count")] uint Count);

        private IFindFluent<Dialog, Dialog> Find(string baseForm, AnalyzerMode analyzerMode) =>
            _mongoCollections[analyzerMode]
                .Find(
                    dialog => dialog.Lines.Any(
                        line => line.Words.Any(word => word.BaseForm == baseForm)
                    )
                );
    }
}