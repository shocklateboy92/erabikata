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
using MongoDB.Driver;
using Newtonsoft.Json;

namespace Erabikata.Backend.CollectionManagers
{
    public class DialogCollectionManager : ICollectionManager
    {
        private readonly IMongoCollection<Dialog> _mongoCollection;
        private readonly AnalyzerService.AnalyzerServiceClient _analyzerServiceClient;
        private readonly SeedDataProvider _seedDataProvider;

        public DialogCollectionManager(
            IMongoDatabase database,
            AnalyzerService.AnalyzerServiceClient analyzerServiceClient,
            SeedDataProvider seedDataProvider)
        {
            _analyzerServiceClient = analyzerServiceClient;
            _seedDataProvider = seedDataProvider;
            _mongoCollection = database.GetCollection<Dialog>(nameof(Dialog));
        }

        public async Task OnActivityExecuting(Activity activity)
        {
            switch (activity)
            {
                case BeginIngestion:
                    await _mongoCollection.DeleteManyAsync(FilterDefinition<Dialog>.Empty);
                    var showFiles = _seedDataProvider.GetShowMetadataFiles();
                    var showTasks = showFiles.Select(
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
                                        var analyzedLines = new List<List<Analyzed>>();
                                        foreach (var line in sentence.Text)
                                        {
                                            var analyzedResponse =await _analyzerServiceClient.AnalyzeTextAsync(
                                                new AnalyzeRequest
                                                {
                                                    Text = line, Mode = AnalyzerMode.SudachiC
                                                }
                                            );
                                        }
                                    }
                                }
                            )
                        }
                    );
                    var resposne = await _analyzerServiceClient.AnalyzeTextAsync(
                        new AnalyzeRequest {Mode = AnalyzerMode.SudachiC, Text = "本当のテクストではありません"}
                    );
                    Console.WriteLine(JsonConvert.SerializeObject(resposne, Formatting.Indented));
                    break;
            }
        }

        public Task InsertMany(IEnumerable<Dialog> dialogs)
        {
            return _mongoCollection.InsertManyAsync(dialogs);
        }
    }
}