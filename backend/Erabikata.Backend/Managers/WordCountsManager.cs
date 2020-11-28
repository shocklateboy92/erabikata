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

            WordRanks = BuildWordCountsSorted(true);
            WordRanksMap = WordRanks.Select((kv, index) => (kv, index))
                .ToDictionary(kvp => kvp.kv.word, kvp => kvp.index);
        }

        public IReadOnlyDictionary<string, int> WordRanksMap { get; }

        public (string word, int count)[] WordRanks { get; }

        public (string word, int count)[] BuildWordCountsSorted(
            bool respectPartOfSpeechFilter,
            IReadOnlyCollection<string>? includePartsOfSpeech = null,
            IReadOnlyCollection<string>? ignoredWords = null)
        {
            var bracketCount = 0;
            var counts = new Dictionary<string, int>();
            foreach (var episode in _database.AllEpisodes)
            foreach (var sentence in episode.Dialog)
            foreach (var word in sentence.Analyzed)
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

                if (includePartsOfSpeech?.Count > 0 &&
                    !word.PartOfSpeech.Any(includePartsOfSpeech.Contains!))
                {
                    continue;
                }

                if (respectPartOfSpeechFilter &&
                    _settings.IgnoredPartsOfSpeech.Any(
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

            return counts.OrderByDescending(kv => kv.Value)
                .Select(kv => (word: kv.Key, count: kv.Value))
                .ToArray();
        }
    }
}