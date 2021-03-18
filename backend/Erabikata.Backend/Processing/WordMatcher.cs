using System.Collections.Generic;
using System.Linq;
using Erabikata.Backend.Stolen;

namespace Erabikata.Backend.Processing
{
    public class WordMatcher
    {
        public record Candidate(
            int WordId,
            IReadOnlyList<IReadOnlyList<string>> NormalizedForms,
            IEnumerable<string[]> DictionaryForms,
            IEnumerable<string> Readings);

        public WordMatcher(IEnumerable<Candidate> candidates)
        {
            foreach (var candidate in candidates)
            {
                AddToTrie(candidate, candidate.NormalizedForms);
                // AddToTrie(candidate, candidate.DictionaryForms);
                AddToTrie(candidate, candidate.Readings.Select(reading => new[] {reading}));
            }

            _trie.Build();
        }

        private void AddToTrie(Candidate candidate, IEnumerable<IReadOnlyList<string>> matchList)
        {
            foreach (var normalized in matchList)
            {
                _trie.Add(normalized, (candidate, normalized.Count));
            }
        }

        private Trie<(Candidate word, int length)> _trie = new();
    }
}