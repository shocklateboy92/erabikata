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
                    var showsToIngest = await Task.WhenAll(
                        _seedDataProvider.GetShowMetadataFilesI()
                            .Select(
                                async path => new IngestShows.ShowToIngest(
                                    // Pretty sure this won't be null, because the file must
                                    // exist for Directory.EnumerateFiles() to find it.
                                    BasePath: Directory.GetParent(path)!.FullName,
                                    Info: await SeedDataProvider.DeserializeFile<ShowInfo>(path)
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