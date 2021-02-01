using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Erabikata.Backend.CollectionManagers;
using Erabikata.Backend.DataProviders;
using Erabikata.Backend.Models.Database;
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
        private readonly DialogCollectionManager _dialogCollectionManager;
        private readonly SubtitleProcessingSettings _settings;
        private readonly SeedDataProvider _seedDataProvider;

        public SubtitleDatabaseManager(
            ILogger<SubtitleDatabaseManager> logger,
            IOptions<SubtitleProcessingSettings> settings,
            DialogCollectionManager dialogCollectionManager,
            SeedDataProvider seedDataProvider)
        {
            _logger = logger;
            _dialogCollectionManager = dialogCollectionManager;
            _seedDataProvider = seedDataProvider;
            _settings = settings.Value;
        }

        public IReadOnlyList<Episode> AllEpisodes { get; private set; } = new Episode[] { };

        public IReadOnlyDictionary<int, EpisodeV2> AllEpisodesV2 { get; private set; } =
            new Dictionary<int, EpisodeV2>();

        public IReadOnlyCollection<ShowInfo> AllShows { get; private set; } =
            new ConcurrentBag<ShowInfo>();

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

            var allShows = new ConcurrentBag<ShowInfo>();

            var newFiles = _seedDataProvider.GetShowMetadataFiles();
            _logger.LogInformation($"Found {newFiles.Count} shows in new format.");
            var readTasks = newFiles.Select(
                    async metadataFile =>
                    {
                        var metaData = JsonConvert.DeserializeObject<ShowInfo>(
                            await File.ReadAllTextAsync(metadataFile)
                        );
                        allShows.Add(metaData);

                        var episodeTasks = metaData.Episodes.First()
                            .Select(
                                async (info, index) =>
                                {
                                    var inputSentences =
                                        JsonConvert.DeserializeObject<InputSentence[]>(
                                            await File.ReadAllTextAsync(
                                                SeedDataProvider.GetDataPath(
                                                    "input",
                                                    metadataFile,
                                                    index
                                                )
                                            )
                                        );
                                    var episodeId = int.Parse(info.Key.Split('/').Last());
                                    return new EpisodeV2
                                    {
                                        Id = episodeId,
                                        Parent = metaData,
                                        Number = index + 1,
                                        AnalyzedSentences =
                                            new Dictionary<Analyzer,
                                                IReadOnlyList<AnalyzedSentenceV2>>
                                            {
                                                [Analyzer.Kuromoji] =
                                                    Array.Empty<AnalyzedSentenceV2>(),
                                                [Analyzer.SudachiA] =
                                                    await DeserializeAnalyzedSentences(
                                                        episodeId,
                                                        AnalyzerMode.SudachiA
                                                    ),
                                                [Analyzer.SudachiB] =
                                                    await DeserializeAnalyzedSentences(
                                                        episodeId,
                                                        AnalyzerMode.SudachiB
                                                    ),
                                                [Analyzer.SudachiC] =
                                                    await DeserializeAnalyzedSentences(
                                                        episodeId,
                                                        AnalyzerMode.SudachiC
                                                    )
                                            },
                                        FilteredInputSentences =
                                            inputSentences
                                                .Select(
                                                    (sentence, i) =>
                                                        new FilteredInputSentence(sentence, i)
                                                )
                                                .Where(sentence => sentence.Text.Any())
                                                .OrderBy(sentence => sentence.Time)
                                                .ToArray(),
                                        EnglishSentences = JsonConvert
                                            .DeserializeObject<InputSentence[]>(
                                                await File.ReadAllTextAsync(
                                                    SeedDataProvider.GetDataPath(
                                                        "english",
                                                        metadataFile,
                                                        index
                                                    )
                                                )
                                            )
                                            .Where(sentence => sentence.Text.Any())
                                            .OrderBy(sentence => sentence.Time)
                                            .ToArray(),
                                        FilePath = info.File
                                    };
                                }
                            )
                            .ToList();
                        await Task.WhenAll(episodeTasks);
                        return episodeTasks.Select(task => task.Result);
                    }
                )
                .ToList();

            await Task.WhenAll(readTasks);
            AllEpisodesV2 = readTasks.SelectMany(task => task.Result)
                .ToDictionary(tuple => tuple.Id);
            AllShows = allShows;
        }

        private async Task<IReadOnlyList<AnalyzedSentenceV2>>
            DeserializeAnalyzedSentences(int episodeId, AnalyzerMode mode) =>
            (await _dialogCollectionManager.GetEpisodeDialog(episodeId, mode)).Select(
                dialog => new AnalyzedSentenceV2(
                    dialog.Lines.Select(
                            list => list.Select(
                                    word => new Analyzed
                                    {
                                        Base = word.BaseForm,
                                        Dictionary = word.DictionaryForm,
                                        Original = word.OriginalForm,
                                        Reading = word.Reading,
                                        PartOfSpeech = word.PartOfSpeech
                                    }
                                )
                                .ToArray()
                        )
                        .ToArray()
                )
            )
            .ToList();
    }
}