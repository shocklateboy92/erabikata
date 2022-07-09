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

namespace Erabikata.Backend.CollectionMiddlewares;

public class DictionaryProviderMiddleware : ICollectionMiddleware
{
    private readonly DatabaseInfoManager _databaseInfo;
    private readonly HttpClient _httpClient;

    public DictionaryProviderMiddleware(HttpClient httpClient, DatabaseInfoManager databaseInfo)
    {
        _httpClient = httpClient;
        _databaseInfo = databaseInfo;
    }

    public async Task Execute(Activity activity, Func<Activity, Task> next)
    {
        if (activity is DictionaryUpdate({ } sourceUrl))
        {
            var currentUrl = await _databaseInfo.GetCurrentDictionary();
            if (currentUrl != sourceUrl)
            {
                var dict = await FetchDictionary(sourceUrl);
                await next(new DictionaryIngestion(ProcessDictionary(dict).ToArray()));
            }

            await _databaseInfo.SetCurrentDictionary(sourceUrl);
        }

        await next(activity);
    }

    private async Task<XElement> FetchDictionary(string sourceUrl)
    {
        var response = await _httpClient.GetAsync(sourceUrl);
        response.EnsureSuccessStatusCode();

        using var stream = new StreamReader(
            new GZipStream(await response.Content.ReadAsStreamAsync(), CompressionMode.Decompress)
        );

        return await XElement.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
    }

    public static IEnumerable<WordInfo> ProcessDictionary(XContainer dictionaryIngestion)
    {
        return dictionaryIngestion
            .Elements("entry")
            .AsParallel()
            .Select(
                entry =>
                    new WordInfo(
                        int.Parse(entry.Element("ent_seq")!.Value),
                        entry.Elements("k_ele").Select(ele => ele.Element("keb")!.Value),
                        entry.Elements("r_ele").Select(ele => ele.Element("reb")!.Value),
                        entry
                            .Elements("sense")
                            .GroupBy(
                                sense =>
                                    sense
                                        .Elements("pos")
                                        .Concat(sense.Elements("misc"))
                                        .Select(element => element.Value)
                                        .Concat(
                                            sense
                                                .Elements("field")
                                                .Select(field => $"{field.Value} term")
                                        )
                                        .ToArray(),
                                (tags, senses) =>
                                    new WordInfo.Meaning(
                                        tags,
                                        senses.Select(
                                            sense =>
                                                string.Join(
                                                    "; ",
                                                    sense
                                                        .Elements("gloss")
                                                        .Select(gloss => gloss.Value)
                                                )
                                        )
                                    ),
                                new EnumerableComparer<string, string[]>()
                            ),
                        entry
                            .Elements("k_ele")
                            .SelectMany(kEle => kEle.Elements("ke_pri"))
                            .Concat(
                                entry.Elements("r_ele").SelectMany(rEle => rEle.Elements("re_pri"))
                            )
                            .Select(ele => ele.Value)
                            .Distinct()
                            .ToHashSet()
                    )
            );
    }
}
