using System;
using System.Threading.Tasks;
using Erabikata.Backend.CollectionManagers;
using Erabikata.Backend.Models.Actions;
using Erabikata.Models.Configuration;
using Microsoft.Extensions.Options;

namespace Erabikata.Backend.CollectionMiddlewares
{
    public class RevisionControlMiddleware : ICollectionMiddleware
    {
        private readonly DatabaseInfoManager _databaseInfoManager;
        private readonly SubtitleProcessingSettings _settings;

        public RevisionControlMiddleware(
            DatabaseInfoManager databaseInfoManager,
            IOptions<SubtitleProcessingSettings> settings
        )
        {
            _databaseInfoManager = databaseInfoManager;
            _settings = settings.Value;
        }

        public async Task Execute(Activity activity, Func<Activity, Task> next)
        {
            switch (activity)
            {
                case BeginIngestion beginIngestion:
                    var previousComment = await _databaseInfoManager.GetCurrentCommit();
                    if (
                        beginIngestion.ForceFullIngestion
                        || previousComment == beginIngestion.StartCommit
                            && (
                                beginIngestion.StartCommit != beginIngestion.EndCommit
                                || _settings.AllowIngestionOfSameCommit
                            )
                    )
                    {
                        await next(beginIngestion);
                    }
                    // Drop the activity at this stage
                    // TODO: Add error reporting / logging
                    break;
                default:
                    await next(activity);
                    break;
            }
        }
    }
}
