using System.Collections.Generic;
using Erabikata.Backend.Stolen;

namespace Erabikata.Backend.Processing
{
    public class WordMatcher
    {
        public record Candidate(
            int WordId,
            IEnumerable<string[]> NormalizedForms,
            IEnumerable<string[]> DictionaryForms,
            IEnumerable<string> Readings)  ;

        public WordMatcher(IEnumerable<Candidate> candidates)
        {
            foreach (var candidate in candidates) { }
        }

        private Trie<Candidate> _trie = new();
    }
}
