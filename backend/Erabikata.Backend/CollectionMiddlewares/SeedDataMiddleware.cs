using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Erabikata.Backend.DataProviders;
using Erabikata.Backend.Models.Actions;
using Erabikata.Models.Input.V2;

namespace Erabikata.Backend.CollectionMiddlewares
{
    public class SeedDataMiddleware : ICollectionMiddleware
    {
        private readonly SeedDataProvider _seedDataProvider;

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
                        filesInSeed.Where(path => path.EndsWith("show-metadata.json"))
                            .Select(
                                async showPath => new IngestShows.ShowToIngest(
                                    Files: filesInSeed.Where(
                                            path => path.StartsWith(
                                                Directory.GetParent(path)!.FullName
                                            )
                                        )
                                        .ToList(),
                                    Info: await SeedDataProvider.DeserializeFile<ShowInfo>(showPath)
                                )
                            )
                    );

                    // Dispatch a new action with the fetched data
                    await next(new IngestShows(showsToIngest));

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
}