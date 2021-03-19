using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Erabikata.Backend.CollectionManagers;
using Erabikata.Backend.Models.Database;
using Erabikata.Backend.Stolen;

namespace Erabikata.Backend.Processing
{
    public class WordMatcher
    {
        public record Candidate(
            int WordId,
            IReadOnlyList<IReadOnlyList<string>> NormalizedForms,
            IEnumerable<string[]> DictionaryForms,
            IEnumerable<string> Readings)
        {
            public ulong Count = 0;
        };

        public WordMatcher(IReadOnlyCollection<Candidate> candidates)
        {
            foreach (var candidate in candidates)
            {
                AddToTrie(candidate, candidate.NormalizedForms);
                // AddToTrie(candidate, candidate.DictionaryForms);
                AddToTrie(candidate, candidate.Readings.Select(reading => new[] {reading}));
            }

            _trie.Build();
            _candidates = candidates;
        }

        private Trie<(Candidate word, int length)> _trie = new();
        private readonly IReadOnlyCollection<Candidate> _candidates;

        private void AddToTrie(Candidate candidate, IEnumerable<IReadOnlyList<string>> matchList)
        {
            foreach (var normalized in matchList)
            {
                _trie.Add(normalized, (candidate, normalized.Count));
            }
        }

        public IReadOnlyCollection<int> FillMatchesAndGetWords(IReadOnlyList<Dialog.Word> words)
        {
            var uniqueMatches = new List<int>();
            var matches = _trie.Find(words);
            foreach (var (endIndex, (word, length)) in matches)
            {
                for (var index = endIndex - length; index < endIndex; index++)
                    words[index].InfoIds.Add(word.WordId);

                Interlocked.Increment(ref word.Count);
                if (!words[endIndex - 1].IsInParenthesis)
                {
                    uniqueMatches.Add(word.WordId);
                }
            }

            return uniqueMatches;
        }

        public IEnumerable<(int WordId, ulong Count)> GetUpdatedWordCounts()
        {
            return _candidates.Select(c => (c.WordId, c.Count));
        }
    }
}