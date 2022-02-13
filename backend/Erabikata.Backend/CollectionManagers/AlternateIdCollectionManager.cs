using System.Linq;
using System.Threading.Tasks;
using Erabikata.Backend.Models.Actions;
using Erabikata.Backend.Models.Database;
using MongoDB.Driver;

namespace Erabikata.Backend.CollectionManagers;

public class AlternateIdCollectionManager : ICollectionManager
{
    private readonly IMongoCollection<AlternateId> _mongoCollection;

    public AlternateIdCollectionManager(IMongoDatabase database)
    {
        _mongoCollection = database.GetCollection<AlternateId>(nameof(AlternateId));
    }

    public Task OnActivityExecuting(Activity activity)
    {
        if (activity is IngestAltShows ingestAltShows)
        {
            // var alternateIds = ingestAltShows.AltShows.SelectMany(
            //     show => new AlternateId(show.));
        }
    }
}
