using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Erabikata.Models;
using Erabikata.Models.Configuration;
using Erabikata.Models.Input;
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

        public SubtitleDatabaseManager(
            ILogger<SubtitleDatabaseManager> logger,
            IOptions<SubtitleProcessingSettings> settings)
        {
            _logger = logger;
            _settings = settings.Value;
        }

        public IReadOnlyList<Episode> AllEpisodes { get; private set; } = new Episode[] { };

        public IReadOnlyDictionary<int, AnalyzedSentenceV2[]> KuoromojiAnalyzedSentenceV2s
        {
            get;
            private set;
        } = new Dictionary<int, AnalyzedSentenceV2[]>();

        public IReadOnlyDictionary<int, InputSentence[]> InputSentences { get; private set; } =
            new Dictionary<int, InputSentence[]>();

        public IReadOnlyDictionary<int, FilteredInputSentence[]> FilteredInputSentences
        {
            get;
            private set;
        } = new Dictionary<int, FilteredInputSentence[]>();

        public IReadOnlyDictionary<int, InputSentence[]> EnglishSentences { get; private set; } =
            new Dictionary<int, InputSentence[]>();

        public IReadOnlyDictionary<int, string> EpisodeFilePaths { get; private set; } =
            new Dictionary<int, string>();

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
                $"Found {files.Length} files in {directories.Length} directories"
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

            var newFiles = Directory.EnumerateFiles(
                    _settings.Input.RootDirectory,
                    "show-metadata.json",
                    SearchOption.AllDirectories
                )
                .ToList();
            _logger.LogInformation($"Found {newFiles.Count} shows in new format.");
            var readTasks = newFiles.Select(
                    async metadataFile =>
                    {
                        var metaData = JsonConvert.DeserializeObject<ShowInfo>(
                            await File.ReadAllTextAsync(metadataFile)
                        );

                        var episodeTasks = metaData.Episodes.First()
                            .Select(
                                async (info, index) => (int.Parse(info.Key.Split('/').Last()),
                                    JsonConvert.DeserializeObject<AnalyzedSentenceV2[]>(
                                        await File.ReadAllTextAsync(
                                            GetDataPath("kuromoji", metadataFile, index)
                                        )
                                    ),
                                    JsonConvert.DeserializeObject<InputSentence[]>(
                                        await File.ReadAllTextAsync(
                                            GetDataPath("input", metadataFile, index)
                                        )
                                    ),
                                    JsonConvert.DeserializeObject<InputSentence[]>(
                                        await File.ReadAllTextAsync(
                                            GetDataPath("english", metadataFile, index)
                                        )
                                    ), info.File)
                            )
                            .ToList();
                        await Task.WhenAll(episodeTasks);
                        return episodeTasks.Select(task => task.Result);
                    }
                )
                .ToList();

            await Task.WhenAll(readTasks);
            var results = readTasks.SelectMany(task => task.Result).ToList();
            KuoromojiAnalyzedSentenceV2s = results.ToDictionary(
                tuples => tuples.Item1,
                tuple => tuple.Item2
            );
            InputSentences = results.ToDictionary(tuple => tuple.Item1, tuple => tuple.Item3);
            FilteredInputSentences = InputSentences.ToDictionary(
                pair => pair.Key,
                pair => pair.Value
                    .Select((sentence, index) => new FilteredInputSentence(sentence, index))
                    .Where(sentence => sentence.Text.Any())
                    .OrderBy(sentence => sentence.Time)
                    .ToArray()
            );
            EnglishSentences = results.ToDictionary(
                tuple => tuple.Item1,
                tuple => tuple.Item4.Where(sentence => sentence.Text.Any())
                    .OrderBy(sentence => sentence.Time)
                    .ToArray()
            );
            EpisodeFilePaths = results.ToDictionary(tuple => tuple.Item1, tuple => tuple.Item5);
        }

        private static string GetDataPath(string dataType, string metadataFile, int index)
        {
            return Path.Combine(metadataFile, $"../{dataType}/{index + 1:00}.json");
        }
    }
}