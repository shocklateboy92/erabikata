using System.Threading.Tasks;
using Erabikata.Backend.Models.Actions;
using Erabikata.Backend.Models.Database;
using MongoDB.Driver;

namespace Erabikata.Backend.CollectionManagers;

public class DatabaseInfoManager : ICollectionManager
{
    private readonly IMongoCollection<DatabaseInfo> _mongoCollection;

    public DatabaseInfoManager(IMongoDatabase mongoDatabase)
    {
        _mongoCollection = mongoDatabase.GetCollection<DatabaseInfo>(nameof(DatabaseInfo));
    }

    public async Task OnActivityExecuting(Activity activity)
    {
        switch (activity)
        {
            case BeginIngestion beginIngestion:
                await _mongoCollection.UpdateOneAsync(
                    FilterDefinition<DatabaseInfo>.Empty,
                    new UpdateDefinitionBuilder<DatabaseInfo>().Set(
                        info => info.IngestedCommit,
                        beginIngestion.EndCommit
                    ),
                    new UpdateOptions { IsUpsert = true }
                );
                break;
        }
    }

    public async Task<string> GetCurrentCommit()
    {
        var dbInfo = await _mongoCollection
            .Find(FilterDefinition<DatabaseInfo>.Empty)
            .FirstOrDefaultAsync();
        return dbInfo?.IngestedCommit ?? string.Empty;
    }

    public Task<string?> GetCurrentDictionary()
    {
        return _mongoCollection
            .Find(FilterDefinition<DatabaseInfo>.Empty)
            .Project(dbInfo => dbInfo.CurrentDictionary)
            .FirstOrDefaultAsync();
    }

    public Task<UpdateResult> SetCurrentDictionary(string sourceUrl)
    {
        return _mongoCollection.UpdateOneAsync(
            FilterDefinition<DatabaseInfo>.Empty,
            new UpdateDefinitionBuilder<DatabaseInfo>().Set(
                info => info.CurrentDictionary,
                sourceUrl
            )
        );
    }
}
