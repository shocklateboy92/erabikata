using System.Collections.Generic;
using System.Linq;
using Erabikata.Models.Configuration;
using Erabikata.Models.Input;
using Microsoft.Extensions.Options;

namespace Erabikata.Backend.Managers
{
    public class WordCountsManager
    {
        private readonly SubtitleDatabaseManager _database;
        private readonly SubtitleProcessingSettings _settings;

        public WordCountsManager(
            IOptions<SubtitleProcessingSettings> settings,
            SubtitleDatabaseManager database)
        {
            _settings = settings.Value;
            _database = database;

            WordRanks =
                new[] {Analyzer.Kuromoji, Analyzer.SudachiA, Analyzer.SudachiB, Analyzer.SudachiC}
                    .ToDictionary(
                        analyzer => analyzer,
                        analyzer => BuildWordCountsSorted(analyzer, respectPartOfSpeechFilter: true)
                    );
            WordRanksMap = WordRanks.ToDictionary(
                ranks => ranks.Key,
                ranks => ranks.Value.Select((kv, index) => (kv, index))
                    .ToDictionary(kvp => kvp.kv.word, kvp => kvp.index)
            );
        }

        public IReadOnlyDictionary<Analyzer, Dictionary<string, int>> WordRanksMap { get; }

        public IReadOnlyDictionary<Analyzer, (string word, int count)[]> WordRanks { get; }

        public (string word, int count)[] BuildWordCountsSorted(
            Analyzer analyzer,
            bool respectPartOfSpeechFilter,
            IReadOnlyCollection<string>? includePartsOfSpeech = null,
            IReadOnlyCollection<string>? ignoredWords = null)
        {
            var counts = new Dictionary<string, int>();
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