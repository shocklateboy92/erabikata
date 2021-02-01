using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Erabikata.Backend.CollectionManagers;
using Erabikata.Models.Configuration;
using Erabikata.Models.Input;
using Microsoft.Extensions.Options;

namespace Erabikata.Backend.Managers
{
    public class WordCountsManager
    {
        private readonly DialogCollectionManager _dialogCollectionManager;
        private readonly SubtitleDatabaseManager _database;
        private readonly SubtitleProcessingSettings _settings;

        public WordCountsManager(
            IOptions<SubtitleProcessingSettings> settings,
            DialogCollectionManager dialogCollectionManager,
            SubtitleDatabaseManager database)
        {
            _settings = settings.Value;
            _dialogCollectionManager = dialogCollectionManager;
            _database = database;
        }

        public async Task Initialize()
        {
            var wordRanks = new Dictionary<AnalyzerMode, List<DialogCollectionManager.WordRank>>();
            foreach (var mode in new[]
            {
                AnalyzerMode.SudachiA, AnalyzerMode.SudachiB, AnalyzerMode.SudachiC
            })
            {
                wordRanks.Add(mode, await _dialogCollectionManager.GetSortedWordCounts(mode));
            }

            WordRanks = wordRanks;

            WordRanksMap = WordRanks.ToDictionary(
                ranks => ranks.Key,
                ranks => ranks.Value.Select((kv, index) => (kv, index))
                    .ToDictionary(kvp => kvp.kv.BaseForm, kvp => kvp.index)
            );
        }

        public IReadOnlyDictionary<AnalyzerMode, Dictionary<string, int>> WordRanksMap
        {
            get;
            private set;
        } = new Dictionary<AnalyzerMode, Dictionary<string, int>>();

        public IReadOnlyDictionary<AnalyzerMode, List<DialogCollectionManager.WordRank>> WordRanks
        {
            get;
            private set;
        } = new Dictionary<AnalyzerMode, List<DialogCollectionManager.WordRank>>();

        public (string word, int count)[] BuildWordCountsSorted(
            Analyzer analyzer,
            bool respectPartOfSpeechFilter,
            IReadOnlyCollection<string>? includePartsOfSpeech = null,
            IReadOnlyCollection<string>? ignoredWords = null)
        {
            var counts = new Dictionary<string, int>();
            if (analyzer == Analyzer.Kuromoji)
            {
                foreach (var episode in _database.AllEpisodes)
                foreach (var sentence in episode.Dialog)
                {
                    CountSentence(
                        respectPartOfSpeechFilter,
                        includePartsOfSpeech,
                        ignoredWords,
                        counts,
                        sentence.Analyzed
                    );
                }
            }

            foreach (var episode in _database.AllEpisodesV2.Values)
            foreach (var sentence in episode.AnalyzedSentences[analyzer])
            foreach (var line in sentence.Analyzed)
            {
                CountSentence(
                    respectPartOfSpeechFilter,
                    includePartsOfSpeech,
                    ignoredWords,
                    counts,
                    line
                );
            }

            return counts.OrderByDescending(kv => kv.Value)
                .Select(kv => (word: kv.Key, count: kv.Value))
                .ToArray();
        }

        private void CountSentence(
            bool respectPartOfSpeechFilter,
            IReadOnlyCollection<string>? includePartsOfSpeech,
            IReadOnlyCollection<string>? ignoredWords,
            Dictionary<string, int> counts,
            IEnumerable<Analyzed> sentence)
        {
            var bracketCount = 0;
            foreach (var word in sentence)
            {
                foreach (var c in word.Original)
                {
                    switch (c)
                    {
                        case '(':
                        case '（':
                            bracketCount++;
                            break;
                        case ')':
                        case '）':
                            bracketCount--;
                            break;
                    }
                }

                if (bracketCount > 0)
                {
                    continue;
                }

                if (includePartsOfSpeech?.Count > 0 && !word.PartOfSpeech.Any<string>(
                    includePartsOfSpeech.Contains!
                ))
                {
                    continue;
                }

                if (respectPartOfSpeechFilter && _settings.IgnoredPartsOfSpeech.Any(
                    ignored => word.PartOfSpeech.Contains(ignored)
                ))
                {
                    continue;
                }

                if (ignoredWords?.Contains(word.Base) ?? false)
                {
                    continue;
                }

                counts[word.Base] = counts.GetValueOrDefault(word.Base) + 1;
            }
        }
    }
}