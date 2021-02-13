using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Erabikata.Backend.CollectionManagers;
using Erabikata.Backend.Models.Actions;
using Microsoft.Extensions.Logging;

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
                    await next(new DictionaryIngestion(dict));
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
    }
}