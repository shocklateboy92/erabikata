using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Erabikata.Backend.DataProviders;
using Erabikata.Backend.Models.Actions;
using Erabikata.Backend.Models.Database;
using Erabikata.Models.Input;
using Erabikata.Models.Input.V2;
using Grpc.Core;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

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
                    await Task.WhenAll(
                        _mongoCollections.SelectMany(
                            pair =>
                            {
                                var (mode, mongoCollection) = pair;
                                return showFiles.Select(
                                    async metadataFilePath =>
                                    {
                                        var metadata = JsonConvert.DeserializeObject<ShowInfo>(
                                            await File.ReadAllTextAsync(metadataFilePath)
                                        );
                                        var episodeTasks = metadata.Episodes.Select(
                                            async (episode, index) =>
                                            {
                                                var inputSentences =
                                                    JsonConvert.DeserializeObject<InputSentence[]>(
                                                        await File.ReadAllTextAsync(
                                                            SeedDataProvider.GetDataPath(
                                                                "input",
                                                                metadataFilePath,
                                                                index
                                                            )
                                                        )
                                                    );

                                                foreach (var sentence in inputSentences)
                                                {
                                                    var analyzedLines =
                                                        new List<IReadOnlyList<Dialog.Word>>();
                                                    foreach (var line in sentence.Text)
                                                    {
                                                        var analyzedResponse =
                                                            await _analyzerServiceClient
                                                                .AnalyzeTextAsync(
                                                                    new AnalyzeRequest
                                                                    {
                                                                        Text = line, Mode = mode
                                                                    }
                                                                );

                                                        analyzedLines.Add(
                                                            analyzedResponse.Words.Select(
                                                                    word => new Dialog.Word(
                                                                        word.BaseForm,
                                                                        word.DictionaryForm
                                                                    ) {Reading = word.Reading}
                                                                )
                                                                .ToList()
                                                        );
                                                    }

                                                    var dialog = new Dialog(
                                                        ObjectId.Empty,
                                                        episode[index].Key.Split('/').Last(),
                                                        sentence.Time
                                                    ) {Time = sentence.Time, Lines = analyzedLines};
                                                    await mongoCollection.InsertOneAsync(dialog);
                                                }
                                            }
                                        );
                                        await Task.WhenAll(episodeTasks);
                                    }
                                );
                            }
                        )
                    );
                    break;
            }
        }

        public Task InsertMany(IEnumerable<Dialog> dialogs)
        {
            // return _mongoCollections.InsertManyAsync(dialogs);
            return Task.CompletedTask;
        }
    }
}