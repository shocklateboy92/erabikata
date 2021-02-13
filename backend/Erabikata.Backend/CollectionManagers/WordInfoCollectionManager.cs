using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Erabikata.Backend.Models.Actions;
using Erabikata.Backend.Models.Database;
using Grpc.Core;
using Grpc.Core.Utils;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MoreLinq;

namespace Erabikata.Backend.CollectionManagers
{
    public class WordInfoCollectionManager : ICollectionManager
    {
        private readonly IMongoCollection<WordInfo> _mongoCollection;
        private readonly ILogger<WordInfoCollectionManager> _logger;
        private readonly AnalyzerService.AnalyzerServiceClient _analyzer;

        public WordInfoCollectionManager(
            IMongoDatabase mongoDatabase,
            ILogger<WordInfoCollectionManager> logger,
            AnalyzerService.AnalyzerServiceClient analyzer)
        {
            _logger = logger;
            _analyzer = analyzer;
            _mongoCollection = mongoDatabase.GetCollection<WordInfo>(nameof(WordInfo));
        }

        public async Task OnActivityExecuting(Activity activity)
        {
            switch (activity)
            {
                case (DictionaryIngestion ({ } dictionary)):
                    await _mongoCollection.DeleteManyAsync(FilterDefinition<WordInfo>.Empty);
                    var words = ProcessDictionary(dictionary).ToList();
                    _logger.LogInformation("Parsing normalized forms");
                    await AddNormalizedForms(words);
                    _logger.LogInformation("Writing to database");
                    await _mongoCollection.InsertManyAsync(
                        words,
                        new InsertManyOptions {IsOrdered = false}
                    );
                    break;
            }
        }

        public static IEnumerable<WordInfo> ProcessDictionary(XContainer dictionaryIngestion) =>
            dictionaryIngestion.Elements("entry")
                .Select(
                    entry => new WordInfo(
                        ObjectId.Empty,
                        entry.Elements("k_ele").Select(ele => ele.Element("keb")!.Value),
                        entry.Elements("r_ele").Select(ele => ele.Element("reb")!.Value)
                    )
                );

        private async Task AddNormalizedForms(IEnumerable<WordInfo> inputWords)
        {
            foreach (var batch in inputWords.Batch(100).Batch(20))
            {
                await Task.WhenAll(
                    batch.Select(
                        async batchWordsInput =>
                        {
                            var batchWords = batchWordsInput.ToList();
                            using var client = _analyzer.AnalyzeDialogBulk();
                            await client.RequestStream.WriteAllAsync(
                                batchWords.Select(
                                    (info, index) => new AnalyzeDialogRequest
                                    {
                                        Lines = {info.Kanji},
                                        Mode = AnalyzerMode.SudachiC,
                                        Style = "none",
                                        Time = index
                                    }
                                )
                            );

                            await foreach (var response in client.ResponseStream.ReadAllAsync())
                            {
                                batchWords[(int) response.Time].Normalized = response.Lines.Select(
                                        line => string.Join(
                                            string.Empty,
                                            line.Words.Select(word => word.BaseForm)
                                        )
                                    )
                                    .Distinct();
                            }
                        }
                    )
                );
            }
        }
    }
}