using System;
using System.Buffers;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Erabikata.Backend.CollectionManagers;
using Erabikata.Backend.Models.Actions;
using Grpc.Core.Logging;
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
            switch (activity)
            {
                case (DictionaryUpdate ({ } sourceUrl)):
                    var currentUrl = await _databaseInfo.GetCurrentDictionary();
                    if (currentUrl == sourceUrl)
                    {
                        break;
                    }

                    await FetchDictionary(sourceUrl);

                    break;
                default:
                    await next(activity);
                    break;
            }
        }

        private async Task FetchDictionary(string sourceUrl)
        {
            var response = await _httpClient.GetAsync(sourceUrl);
            response.EnsureSuccessStatusCode();

            using var stream = new StreamReader(
                new GZipStream(
                    await response.Content.ReadAsStreamAsync(),
                    CompressionMode.Decompress
                )
            );
            var str = await stream.ReadToEndAsync();
            _logger.LogInformation(string.Join("\n", response.Headers));
            _logger.LogInformation(string.Join("\n", str.Split().Take(10)));
        }
    }
}