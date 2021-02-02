using System.Threading.Tasks;
using Erabikata.Backend.DataProviders;
using Erabikata.Backend.Models.Actions;

namespace Erabikata.Backend.CollectionManagers
{
    public class EngSubCollectionManager : ICollectionManager
    {
        private readonly SeedDataProvider _seedDataProvider;

        public EngSubCollectionManager(SeedDataProvider seedDataProvider)
        {
            _seedDataProvider = seedDataProvider;
        }

        public async Task OnActivityExecuting(Activity activity)
        {
            switch (activity)
            {
                case BeginIngestion:
                    await IngestEngSubs();
                    break;
            }
        }

        private async Task IngestEngSubs()
        {
            
        }
    }
}