using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Erabikata.Backend.Models.Actions;
using Erabikata.Backend.Models.Database;
using Grpc.Core;
using Grpc.Core.Utils;
using MoreLinq.Extensions;

namespace Erabikata.Backend.CollectionMiddlewares
{
    public class WordInfoNormalizeMiddleware : ICollectionMiddleware
    {
        private readonly AnalyzerService.AnalyzerServiceClient _analyzer;

        public WordInfoNormalizeMiddleware(AnalyzerService.AnalyzerServiceClient analyzer)
        {
            _analyzer = analyzer;
        }

        public async Task Execute(Activity activity, Func<Activity, Task> next)
        {
            if (activity is DictionaryIngestion ({ } words))
            {
                // mutates inline, because this is already memory intensive
                await AddNormalizedForms(words);
            }

            await next(activity);
        }

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
                                        Lines = {info.Kanji.Any() ? info.Kanji : info.Readings},
                                        Mode = AnalyzerMode.SudachiC,
                                        Style = "none",
                                        Time = index
                                    }
                                )
                            );

                            await foreach (var response in client.ResponseStream.ReadAllAsync())
                            {
                                batchWords[(int) response.Time].Normalized = response.Lines.Select(
                                        line => line.Words.Select(word => word.BaseForm).ToList()
                                    )
                                    .ToList();
                            }
                        }
                    )
                );
            }
        }
    }
}