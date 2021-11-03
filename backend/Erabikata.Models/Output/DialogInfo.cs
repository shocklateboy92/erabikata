using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

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

        [JsonProperty(Required = Required.Always)]
        public string Id { get; set; }

        [JsonProperty(Required = Required.Always)]
        public double StartTime { get; }

        [JsonProperty(Required = Required.Always)]
        public WordRef[][] Words { get; }

        [JsonProperty(Required = Required.Always)]
        public bool IsSongLyric { get; }

        public class WordRef
        {
            public WordRef(
                string displayText,
                string baseForm,
                string reading,
                IEnumerable<int> wordInfoIds)
            {
                DisplayText = displayText;
                BaseForm = baseForm;
                Reading = string.Join(
                    string.Empty,
                    reading.Select(c => c >= 'ァ' && c <= 'ヴ' ? (char) (c - 0x0060) : c)
                );
                DefinitionIds = wordInfoIds.Distinct();
            }

            [JsonProperty(Required = Required.Always)]
            public string DisplayText { get; }

            [JsonProperty(Required = Required.Always)]
            public string BaseForm { get; }

            [JsonProperty(Required = Required.Always)]
            public string Reading { get; }

            [JsonProperty(Required = Required.Always)]
            public IEnumerable<int> DefinitionIds { get; set; }
        }
    }
#nullable restore
}
