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
                    var showFiles = _seedDataProvider.GetShowMetadataFiles();
                    var tasks = await Task.WhenAll(
                        showFiles.Select(
                            async metadataFilePath =>
                            {
                                var metadata = await DeserializeFile<ShowInfo>(metadataFilePath);
                                return metadata.Episodes.Select(
                                    (_, index) => new
                                    {
                                        path = SeedDataProvider.GetDataPath(
                                            "input",
                                            metadataFilePath,
                                            index
                                        ),
                                        id = metadata.Key.Split('/').Last(),
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
                                            response =>
                                                new Dialog(ObjectId.Empty, job.id, response.Time)
                                                {
                                                    Lines = response.Lines.Select(
                                                            line => line.Words.Select(
                                                                    word => new Dialog.Word(
                                                                        word.BaseForm,
                                                                        word.DictionaryForm
                                                                    ) {Reading = word.Reading}
                                                                )
                                                                .ToList()
                                                        )
                                                        .ToList()
                                                }
                                        )
                                    );
                                }
                            );
                        await Task.WhenAll(jobs);
                    }


                    break;
            }
        }

        public Task InsertMany(IEnumerable<Dialog> dialogs)
        {
            // return _mongoCollections.InsertManyAsync(dialogs);
            return Task.CompletedTask;
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
    }
}