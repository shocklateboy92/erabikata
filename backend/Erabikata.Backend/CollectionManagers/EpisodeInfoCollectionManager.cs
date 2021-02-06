using System.Linq;
using System.Threading.Tasks;
using Erabikata.Backend.Models.Actions;
using Erabikata.Backend.Models.Database;
using Mapster;
using MongoDB.Driver;

namespace Erabikata.Backend.CollectionManagers
{
    public class EpisodeInfoCollectionManager : ICollectionManager
    {
        private readonly IMongoCollection<EpisodeInfo> _mongoCollection;

        public EpisodeInfoCollectionManager(IMongoDatabase mongoDatabase)
        {
            _mongoCollection = mongoDatabase.GetCollection<EpisodeInfo>(nameof(EpisodeInfo));
        }

        public async Task OnActivityExecuting(Activity activity)
        {
            switch (activity)
            {
                case IngestShows ingestShows:
                    await _mongoCollection.DeleteManyAsync(FilterDefinition<EpisodeInfo>.Empty);
                    await _mongoCollection.InsertManyAsync(
                        ingestShows.ShowsToIngest.SelectMany(
                            ingest => ingest.Info.Episodes[0]
                                .Select(
                                    info => new EpisodeInfo(
                                        int.Parse(info.Key.Split('/').Last()),
                                        info.File
                                    )
                                )
                        )
                    );
                    break;
            }
        }

        public Task<string?> GetFilePathOfEpisode(int episodeId) =>
            _mongoCollection.Find(info => info.Id == episodeId)
                .Project(info => info.File)
                .FirstOrDefaultAsync()!;
    }
}