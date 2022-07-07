using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Erabikata.Backend.DataProviders;
using Erabikata.Backend.Models.Actions;
using Erabikata.Backend.Models.Input;

namespace Erabikata.Backend.CollectionMiddlewares;

public class SeedDataMiddleware : ICollectionMiddleware
{
    private const string ShowMetadataJsonFileName = "/show-metadata.json";
    private readonly SeedDataProvider _seedDataProvider;
    private static readonly Regex ServerPrefixRegex = new(
        "/([^/]+)_show-metadata.json",
        RegexOptions.Compiled
    );

    public SeedDataMiddleware(SeedDataProvider seedDataProvider)
    {
        _seedDataProvider = seedDataProvider;
    }

    public async Task Execute(Activity activity, Func<Activity, Task> next)
    {
        switch (activity)
        {
            case BeginIngestion:
                var filesInSeed = _seedDataProvider.GetAllFiles();
                var showsToIngest = await Task.WhenAll(
                    filesInSeed
                        .Where(path => path.EndsWith(ShowMetadataJsonFileName))
                        .Select(
                            async showPath =>
                                new IngestShows.ShowToIngest(
                                    filesInSeed
                                        .Where(
                                            path =>
                                                path.StartsWith(
                                                    showPath.Replace(
                                                        ShowMetadataJsonFileName,
                                                        string.Empty
                                                    )
                                                )
                                        )
                                        .ToList(),
                                    await SeedDataProvider.DeserializeFile<ShowInfo>(showPath)
                                )
                        )
                );

                // Dispatch a new action with the fetched data
                await next(new IngestShows(showsToIngest));

                var altShowsToIngest = await Task.WhenAll(
                    showsToIngest.SelectMany(
                        show =>
                            show.Files
                                .Where(path => path.EndsWith("_show-metadata.json"))
                                .Select(
                                    async path =>
                                        new AltShow(
                                            ServerPrefixRegex
                                                .Match(path)
                                                .Groups.Values.Skip(1)
                                                .FirstOrDefault()
                                                ?.Value ?? string.Empty,
                                            Info: await SeedDataProvider.DeserializeFile<ShowInfo>(
                                                path
                                            ),
                                            Original: show.Info
                                        )
                                )
                    )
                );
                await next(new IngestAltShows(altShowsToIngest));

                // Also continue the original action, so the db info manager
                // updates its state correctly.
                await next(activity);
                break;
            default:
                await next(activity);
                break;
        }
    }
}
