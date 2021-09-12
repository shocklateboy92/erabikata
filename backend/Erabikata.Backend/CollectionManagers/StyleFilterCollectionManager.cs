using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Erabikata.Backend.Extensions;
using Erabikata.Backend.Models.Actions;
using Erabikata.Backend.Models.Database;
using MongoDB.Driver;

namespace Erabikata.Backend.CollectionManagers
{
    public class StyleFilterCollectionManager : ICollectionManager
    {
        private readonly IMongoCollection<StyleFilter> _mongoCollection;

        public StyleFilterCollectionManager(IMongoDatabase mongoDatabase)
        {
            _mongoCollection = mongoDatabase.GetCollection<StyleFilter>(nameof(StyleFilter));
        }

        public async Task OnActivityExecuting(Activity activity)
        {
            switch (activity)
            {
                case IngestShows ingestShows:
                    var existingShows = await _mongoCollection.Find(
                            FilterDefinition<StyleFilter>.Empty
                        )
                        .Project(filter => filter.ShowId)
                        .ToListAsync();
                    var showIdMap = existingShows.ToHashSet();
                    var toInsert = await Task.WhenAll(
                        ingestShows.ShowsToIngest.Where(
                                show => !showIdMap.Contains(show.Info.Key.ParseId())
                            )
                            .Select(
                                async show =>
                                {
                                    var file = show.Files.FirstOrDefault(
                                        name => name.EndsWith("english/include_styles.txt")
                                    );
                                    var styles =
                                        file == null
                                            ? Enumerable.Empty<string>()
                                            : await File.ReadAllLinesAsync(file);

                                    return new StyleFilter(
                                        show.Info.Key.ParseId(),
                                        styles,
                                        show.Info.Episodes[0].Select(
                                            episode => episode.Key.ParseId()
                                        )
                                    );
                                }
                            )
                    );

                    if (toInsert.Any())
                    {
                        await _mongoCollection.InsertManyAsync(toInsert);
                    }
                    break;
                case EnableStyle toEnable:
                    await _mongoCollection.UpdateOneAsync(
                        filter => filter.ShowId == toEnable.ShowId,
                        Builders<StyleFilter>.Update.AddToSet(
                            filter => filter.EnabledStyles,
                            toEnable.StyleName
                        )
                    );
                    break;
                case DisableStyle toDisable:
                    await _mongoCollection.UpdateOneAsync(
                        filter => filter.ShowId == toDisable.ShowId,
                        Builders<StyleFilter>.Update.Pull(
                            filter => filter.EnabledStyles,
                            toDisable.StyleName
                        )
                    );
                    break;
            }
        }

        public Task<IEnumerable<string>> GetActiveStylesFor(int episode)
        {
            return _mongoCollection.Find(filter => filter.ForEpisodes.Contains(episode))
                .Project(filter => filter.EnabledStyles)
                .FirstOrDefaultAsync();
        }

        public async Task<StyleFilter?> GetByShowId(int showId) =>
            await _mongoCollection.Find(filter => filter.ShowId == showId).FirstOrDefaultAsync();

        public async Task<int?> GetShowIdOf(int episodeId)
        {
            return await _mongoCollection.Find(filter => filter.ForEpisodes.Contains(episodeId))
                .Project(filter => filter.ShowId)
                .FirstOrDefaultAsync();
        }
    }
}
