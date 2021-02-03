using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Erabikata.Backend.DataProviders;
using Erabikata.Models.Configuration;
using Erabikata.Models.Input.V2;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Erabikata.Backend.Managers
{
    public class SubtitleDatabaseManager
    {
        private readonly ILogger<SubtitleDatabaseManager> _logger;
        private readonly SubtitleProcessingSettings _settings;
        private readonly SeedDataProvider _seedDataProvider;

        public SubtitleDatabaseManager(
            ILogger<SubtitleDatabaseManager> logger,
            IOptions<SubtitleProcessingSettings> settings,
            SeedDataProvider seedDataProvider)
        {
            _logger = logger;
            _seedDataProvider = seedDataProvider;
            _settings = settings.Value;
        }

        public IReadOnlyDictionary<int, EpisodeV2> AllEpisodesV2 { get; private set; } =
            new Dictionary<int, EpisodeV2>();

        public async Task Initialize()
        {
            _logger.LogInformation(
                "Initializing subtitle database from '{RootDirectory}'",
                _settings.Input.RootDirectory
            );
            var allShows = new ConcurrentBag<ShowInfo>();

            var newFiles = _seedDataProvider.GetShowMetadataFilesI().ToList();
            _logger.LogInformation("Found {Count} shows in new format", newFiles.Count);
            var readTasks = newFiles.Select(
                    async metadataFile =>
                    {
                        var metaData = JsonConvert.DeserializeObject<ShowInfo>(
                            await File.ReadAllTextAsync(metadataFile)
                        );
                        allShows.Add(metaData);

                        return metaData.Episodes[0]
                            .Select(
                                (info, index) =>
                                {
                                    var episodeId = int.Parse(info.Key.Split('/').Last());
                                    return new EpisodeV2
                                    {
                                        Id = episodeId,
                                        Parent = metaData,
                                        Number = index + 1,
                                        FilePath = info.File
                                    };
                                }
                            )
                            .ToList();
                    }
                )
                .ToList();

            await Task.WhenAll(readTasks);
            AllEpisodesV2 = readTasks.SelectMany(task => task.Result)
                .ToDictionary(tuple => tuple.Id);
        }
    }
}