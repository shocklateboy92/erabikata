using System.Collections.Generic;
using System.Threading.Tasks;
using Erabikata.Backend.Models.Actions;
using Erabikata.Backend.Models.Database;
using MongoDB.Driver;

namespace Erabikata.Backend.CollectionManagers
{
    public class DialogCollectionManager : ICollectionManager
    {
        private readonly IMongoCollection<Dialog> _mongoCollection;

        public DialogCollectionManager(IMongoDatabase database)
        {
            _mongoCollection = database.GetCollection<Dialog>(nameof(Dialog));
        }

        public async Task OnActivityExecuting(Activity activity)
        {
            switch (activity)
            {
                case BeginIngestion:
                    await _mongoCollection.DeleteManyAsync(FilterDefinition<Dialog>.Empty);
                    break;
            }
        }

        public Task InsertMany(IEnumerable<Dialog> dialogs)
        {
            return _mongoCollection.InsertManyAsync(dialogs);
        }
    }
}