using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Erabikata.Backend.CollectionManagers;
using Erabikata.Backend.Models.Actions;
using Erabikata.Backend.Models.Database;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace Erabikata.Backend.CollectionMiddlewares
{
    public class DictionaryProviderMiddleware : ICollectionMiddleware
    {
        private readonly HttpClient _httpClient;
        private readonly DatabaseInfoManager _databaseInfo;
        private readonly ILogger<DictionaryProviderMiddleware> _logger;

        public DictionaryProviderMiddleware(
            HttpClient httpClient,
            DatabaseInfoManager databaseInfo,
            ILogger<DictionaryProviderMiddleware> logger)
        {
            _httpClient = httpClient;
            _databaseInfo = databaseInfo;
            _logger = logger;
        }

        public async Task Execute(Activity activity, Func<Activity, Task> next)
        {
            if (activity is DictionaryUpdate ({ } sourceUrl))
            {
                var currentUrl = await _databaseInfo.GetCurrentDictionary();
                if (currentUrl != sourceUrl)
                {
                    var dict = await FetchDictionary(sourceUrl);
                    await next(new DictionaryIngestion(ProcessDictionary(dict).ToList()));
                }
            }

            await next(activity);
        }

        private async Task<XElement> FetchDictionary(string sourceUrl)
        {
            var response = await _httpClient.GetAsync(sourceUrl);
            response.EnsureSuccessStatusCode();

            using var stream = new StreamReader(
                new GZipStream(
                    await response.Content.ReadAsStreamAsync(),
                    CompressionMode.Decompress
                )
            );

            return await XElement.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
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
    }
}