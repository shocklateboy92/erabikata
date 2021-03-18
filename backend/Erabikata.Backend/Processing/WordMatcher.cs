using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        private Trie<(Candidate word, int length)> _trie = new();

        private void AddToTrie(Candidate candidate, IEnumerable<IReadOnlyList<string>> matchList)
        {
            foreach (var normalized in matchList)
            {
                _trie.Add(normalized, (candidate, normalized.Count));
            }
        }

        public void FillMatches(Dialog.Word[] words)
        {
            var matches = _trie.Find(words);
            foreach (var (endIndex, (word, length)) in matches)
            {
                for (var index = endIndex - length; index < endIndex; index++)
                    words[index].InfoIds.Add(word.Id);

                Interlocked.Increment(ref word.Count);
                if (!words[endIndex - 1].IsInParenthesis)
                {
                    dialog.WordsToRank.Add(word.Id);
                }
            }
        }
    }
}