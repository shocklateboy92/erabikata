using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Erabikata.Backend.Extensions;
using Erabikata.Backend.Models.Actions;
using Erabikata.Backend.Models.Database;
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
                    var toInsert = await Task.WhenAll(
                        ingestShows.ShowsToIngest.Select(
                            async ingest =>
                            {
                                var tracksFile = ingest.Files.FirstOrDefault(
                                    path => path.EndsWith("english/include_tracks.txt")
                                );
                                var subTracks =
                                    tracksFile == null
                                        ? null
                                        : await File.ReadAllLinesAsync(tracksFile);
                                return ingest.Info.Episodes[0].Select(
                                    info =>
                                        new EpisodeInfo(
                                            info.Key.ParseId(),
                                            info.File,
                                            subTracks?.ToHashSet()
                                        )
                                );
                            }
                        )
                    );
                    await _mongoCollection.InsertManyAsync(toInsert.SelectMany(a => a));
                    break;
            }
        }

        public async Task<EpisodeInfo?> GetEpisodeInfo(int episodeId)
        {
            return await _mongoCollection.Find(info => info.Id == episodeId).FirstOrDefaultAsync();
        }
    }
}
