using System.Linq;
using System.Threading.Tasks;
using Erabikata.Backend.Extensions;
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

    public async Task OnActivityExecuting(Activity activity)
    {
        if (activity is IngestAltShows ingestAltShows)
        {
            var alternateIds = ingestAltShows.AltShows
                .SelectMany(
                    show =>
                        show.Info.Episodes[0]
                            .Select(
                                (ep, index) =>
                                    new AlternateId(
                                        ep.Key.ParseId(),
                                        show.Original.Episodes[0][index].Key.ParseId(),
                                        show.Prefix,
                                        AlternateIdType.Episode
                                    )
                            )
                )
                .ToList();
            alternateIds.AddRange(
                ingestAltShows.AltShows.Select(
                    show =>
                        new AlternateId(
                            show.Info.Key.ParseId(),
                            show.Original.Key.ParseId(),
                            show.Prefix,
                            AlternateIdType.Show
                        )
                )
            );

            await _mongoCollection.DeleteManyAsync(FilterDefinition<AlternateId>.Empty);
            await _mongoCollection.InsertManyAsync(alternateIds);
        }
    }
}
