using System;
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
            IReadOnlyList<IReadOnlyList<string>> DictionaryForms)
        {
            public ulong Count = 0;

            public virtual bool Equals(Candidate? other) => other?.WordId == WordId;
            public override int GetHashCode() => WordId.GetHashCode();
        };

        public WordMatcher(IReadOnlyCollection<Candidate> candidates)
        {
            foreach (var candidate in candidates)
            {
                AddToTrie(candidate, candidate.DictionaryForms);
                AddToTrie(candidate, candidate.NormalizedForms);
            }

            _trie.Build();
            _candidates = candidates;
        }

        private readonly Trie<(Candidate word, int length)> _trie = new();
        private readonly IReadOnlyCollection<Candidate> _candidates;

        private void AddToTrie(Candidate candidate, IEnumerable<IReadOnlyList<string>> matchList)
        {
            foreach (var normalized in matchList)
            {
                _trie.Add(normalized, (candidate, normalized.Sum(s => s.Length)));
            }
        }

        public IEnumerable<int> FillMatchesAndGetWords(IReadOnlyList<Dialog.Word> words)
        {
            var uniqueMatches = new HashSet<Candidate>();
            ProcessMatchesOfType(
                words,
                uniqueMatches,
                _trie.Find(words.Select(word => word.DictionaryForm)),
                word => word.DictionaryForm
            );
            ProcessMatchesOfType(
                words,
                uniqueMatches,
                _trie.Find(words.Select(word => word.BaseForm)),
                word => word.BaseForm
            );

            foreach (var candidate in uniqueMatches)
            {
                Interlocked.Increment(ref candidate.Count);
            }

            return uniqueMatches.Select(candidate => candidate.WordId);
        }

        private static void ProcessMatchesOfType(
            IReadOnlyList<Dialog.Word> words,
            ISet<Candidate> uniqueMatches,
            IEnumerable<(int index, (Candidate word, int length) value)> matches,
            Func<Dialog.Word, string> formSelector)
        {
            foreach (var (endIndex, (word, length)) in matches)
            {
                var index = endIndex - 1;
                var remainingLength = length;
                while (remainingLength > 0)
                {
                    words[index].InfoIds.Add(word.WordId);

                    remainingLength -= formSelector(words[index]).Length;
                    index--;
                }

                if (!words[endIndex - 1].IsInParenthesis)
                {
                    uniqueMatches.Add(word);
                }
            }
        }

        public IEnumerable<(int WordId, ulong Count)> GetUpdatedWordCounts()
        {
            return _candidates.Select(c => (c.WordId, c.Count));
        }
    }
}