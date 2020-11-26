using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Erabikata.Models;
using Erabikata.Models.Configuration;
using Erabikata.Models.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Erabikata.Backend.Managers
{
    public class SubtitleDatabaseManager
    {
        private readonly ILogger<SubtitleDatabaseManager> _logger;
        private readonly SubtitleProcessingSettings _settings;

        public SubtitleDatabaseManager(
            ILogger<SubtitleDatabaseManager> logger,
            IOptions<SubtitleProcessingSettings> settings)
        {
            _logger = logger;
            _settings = settings.Value;
        }

        public IReadOnlyList<Episode> AllEpisodes { get; private set; } = new Episode[] { };

        public async Task Initialize()
        {
            _logger.LogInformation(
                $"Initializing subtitle database from '{_settings.Input.RootDirectory}'"
            );
            var directories = Directory.GetDirectories(
                _settings.Input.RootDirectory,
                _settings.Input.DirectoryFilter,
                SearchOption.AllDirectories
            );

            var files = directories
                .SelectMany(dir => Directory.EnumerateFiles(dir, _settings.Input.FileFilter))
                .ToArray();

            _logger.LogInformation(
                $"Found {files.Count()} files in {directories.Length} directories"
            );

            var parseTasks = files.Select(
                    async file => new Episode
                    {
                        Title = file, Dialog = Sentence.FromJson(await File.ReadAllTextAsync(file))
                    }
                )
                .ToArray();

            // Make all the file reads async to hopefully parallelize them
            await Task.WhenAll(parseTasks);

            AllEpisodes = parseTasks.Select(t => t.Result)
                .Where(t => t.Dialog.Any(w => w.Analyzed.Any()))
                .ToImmutableArray();
        }
    }
}