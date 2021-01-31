using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
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
                    await Task.WhenAll(
                        _mongoCollections.SelectMany(
                            pair =>
                            {
                                var (mode, mongoCollection) = pair;
                                return showFiles.AsParallel()
                                    .Select(
                                        async metadataFilePath =>
                                        {
                                            await using var file = File.OpenRead(metadataFilePath);
                                            var metadata =
                                                await JsonSerializer.DeserializeAsync<ShowInfo>(
                                                    file,
                                                    new JsonSerializerOptions
                                                    {
                                                        PropertyNamingPolicy =
                                                            JsonNamingPolicy.CamelCase
                                                    }
                                                );
                                            for (var index = 0;
                                                index < metadata!.Episodes.Count;
                                                index++)
                                            {
                                                await IngestEpisode(
                                                    metadataFilePath,
                                                    index,
                                                    mode,
                                                    metadata.Episodes[index],
                                                    mongoCollection
                                                );
                                            }
                                        }
                                    );
                            }
                        )
                    );
                    break;
            }
        }

        private async Task IngestEpisode(
            string metadataFilePath,
            int index,
            AnalyzerMode mode,
            IReadOnlyList<ShowInfo.EpisodeInfo> episode,
            IMongoCollection<Dialog> mongoCollection)
        {
            await using var fileStream = File.OpenRead(
                SeedDataProvider.GetDataPath("input", metadataFilePath, index)
            );
            var inputSentences = await JsonSerializer.DeserializeAsync<InputSentence[]>(
                fileStream,
                new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase}
            );

            var outputSentences = new List<Dialog>();
            foreach (var sentence in inputSentences!)
            {
                var analyzedLines = new List<IReadOnlyList<Dialog.Word>>();
                foreach (var line in sentence.Text)
                {
                    var analyzedResponse =
                        await _analyzerServiceClient.AnalyzeTextAsync(
                            new AnalyzeRequest {Text = line, Mode = mode}
                        );

                    analyzedLines.Add(
                        analyzedResponse.Words.Select(
                                word => new Dialog.Word(word.BaseForm, word.DictionaryForm)
                                {
                                    Reading = word.Reading
                                }
                            )
                            .ToList()
                    );
                }

                outputSentences.Add(
                    new Dialog(ObjectId.Empty, episode[index].Key.Split('/').Last(), sentence.Time)
                    {
                        Time = sentence.Time, Lines = analyzedLines
                    }
                );
            }

            await mongoCollection.InsertManyAsync(outputSentences);
        }

        public Task InsertMany(IEnumerable<Dialog> dialogs)
        {
            // return _mongoCollections.InsertManyAsync(dialogs);
            return Task.CompletedTask;
        }
    }
}