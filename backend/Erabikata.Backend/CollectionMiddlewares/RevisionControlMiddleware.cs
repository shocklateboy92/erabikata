using System;
using System.Threading.Tasks;
using Erabikata.Backend.CollectionManagers;
using Erabikata.Backend.Models.Actions;

namespace Erabikata.Backend.CollectionMiddlewares
{
    public class RevisionControlMiddleware : ICollectionMiddleware
    {
        private readonly DatabaseInfoManager _databaseInfoManager;

        public RevisionControlMiddleware(DatabaseInfoManager databaseInfoManager)
        {
            _databaseInfoManager = databaseInfoManager;
        }

        public async Task Execute(Activity activity, Func<Activity, Task> next)
        {
            switch (activity)
            {
                case BeginIngestion beginIngestion:
                    var previousComment = await _databaseInfoManager.GetCurrentCommit();
                    if (previousComment == beginIngestion.StartCommit &&
                        beginIngestion.StartCommit != beginIngestion.EndCommit)
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