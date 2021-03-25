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
            IReadOnlyList<IReadOnlyList<string>> DictionaryForms,
            IEnumerable<string> Kanji)
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

                foreach (var kanji in candidate.Kanji)
                {
                    if (kanji.Length > 1)
                    {
                        _charTrie.Add(kanji, (candidate, kanji.Length));
                    }
                }
            }

            _wordTrie.Build();
            _charTrie.Build();
            _candidates = candidates;
        }

        private readonly Trie<(Candidate word, int length), string> _wordTrie = new();
        private readonly Trie<(Candidate word, int length), char> _charTrie = new();
        private readonly IReadOnlyCollection<Candidate> _candidates;

        private void AddToTrie(Candidate candidate, IEnumerable<IReadOnlyList<string>> matchList)
        {
            foreach (var normalized in matchList)
            {
                _wordTrie.Add(normalized, (candidate, normalized.Count));
            }
        }

        public IEnumerable<int> FillMatchesAndGetWords(IReadOnlyList<Dialog.Word> words)
        {
            var uniqueMatches = new HashSet<Candidate>();
            var dictionaryForms = words.Select(word => word.DictionaryForm).ToArray();
            ProcessMatchesOfType(words, uniqueMatches, _wordTrie.Find(dictionaryForms));
            ProcessMatchesOfType(
                words,
                uniqueMatches,
                _wordTrie.Find(words.Select(word => word.BaseForm))
            );

            ProcessCharacterMatches(
                words,
                uniqueMatches,
                _charTrie.Find(dictionaryForms.SelectMany(s => s)),
                dictionaryForms
            );

            foreach (var candidate in uniqueMatches)
            {
                Interlocked.Increment(ref candidate.Count);
            }

            return uniqueMatches.Select(candidate => candidate.WordId);
        }

        private static void ProcessCharacterMatches(
            IReadOnlyList<Dialog.Word> words,
            ISet<Candidate> uniqueMatches,
            IEnumerable<(int index, (Candidate word, int length) value)> matches,
            IReadOnlyList<string> wordMatchForms)
        {
            // Since the matches have a char index yet input word forms
            // are strings, build a lookup/map to easily convert back.
            var wordCharMap = new int[wordMatchForms.Sum(word => word.Length)];
            var formIndex = 0;
            var currentFormLength = wordMatchForms[formIndex].Length;
            for (var i = 0; i < wordCharMap.Length; i++)
            {
                if (currentFormLength == 0)
                {
                    formIndex++;
                    currentFormLength = wordMatchForms[formIndex].Length;
                }

                wordCharMap[i] = formIndex;

                currentFormLength--;
            }

            // Use the map to assign match ids to the word objects.
            foreach (var (matchIndex, (word, length)) in matches)
            {
                var wordIndexes = wordCharMap.Skip(matchIndex - length).Take(length).Distinct();
                foreach (var wordIndex in wordIndexes)
                {
                    words[wordIndex].InfoIds.Add(word.WordId);

                    if (!words[wordIndex].IsInParenthesis)
                    {
                        uniqueMatches.Add(word);
                    }
                }
            }
        }

        private static void ProcessMatchesOfType(
            IReadOnlyList<Dialog.Word> words,
            ISet<Candidate> uniqueMatches,
            IEnumerable<(int index, (Candidate word, int length) value)> matches)
        {
            foreach (var (endIndex, (word, length)) in matches)
            {
                for (var index = endIndex - length; index < endIndex; index++)
                    words[index].InfoIds.Add(word.WordId);

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