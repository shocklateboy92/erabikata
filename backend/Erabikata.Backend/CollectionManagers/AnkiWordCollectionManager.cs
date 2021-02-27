using System.Threading.Tasks;
using Erabikata.Backend.Models;
using Erabikata.Backend.Models.Actions;
using Erabikata.Backend.Models.Database;
using MongoDB.Driver;

namespace Erabikata.Backend.CollectionManagers
{
    public class AnkiWordCollectionManager : ICollectionManager
    {
        private readonly IAnkiSyncClient _ankiSyncClient;
        private readonly IMongoCollection<AnkiWord> _mongoCollection;

        public AnkiWordCollectionManager(IAnkiSyncClient ankiSyncClient, IMongoDatabase database)
        {
            _ankiSyncClient = ankiSyncClient;
            _mongoCollection = database.GetCollection<AnkiWord>(nameof(AnkiWord));
        }

        public async Task OnActivityExecuting(Activity activity)
        {
            switch (activity)
            {
                case SyncAnki:
                    await _mongoCollection.DeleteManyAsync(FilterDefinition<AnkiWord>.Empty);
                    var response = await _ankiSyncClient.FindNotes(
                        new IAnkiSyncClient.FindNotesAction("\"note:Jap Sentences 2\"")
                    );
                    break;
            }
        }
    }
}