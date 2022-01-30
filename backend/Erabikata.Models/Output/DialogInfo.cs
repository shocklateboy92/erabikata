using System.Collections.Generic;
using System.Linq;

namespace Erabikata.Models.Output
{
#nullable enable
    public class DialogInfo
    {
        public DialogInfo(string id, double startTime, WordRef[][] tokenized, bool isSongLyric)
        {
            StartTime = startTime;
            Words = tokenized;
            Id = id;
            IsSongLyric = isSongLyric;
        }

        public string Id { get; set; }

        public double StartTime { get; }

        public WordRef[][] Words { get; }

        public bool IsSongLyric { get; }

        public class WordRef
        {
            public WordRef(
                string displayText,
                string baseForm,
                string reading,
                IEnumerable<int> wordInfoIds
            )
            {
                DisplayText = displayText;
                BaseForm = baseForm;
                Reading = string.Join(
                    string.Empty,
                    reading.Select(c => c >= 'ァ' && c <= 'ヴ' ? (char)(c - 0x0060) : c)
                );
                DefinitionIds = wordInfoIds.Distinct();
            }

            public string DisplayText { get; }

            public string BaseForm { get; }

            public string Reading { get; }

            public IEnumerable<int> DefinitionIds { get; set; }
        }
    }
#nullable restore
}
