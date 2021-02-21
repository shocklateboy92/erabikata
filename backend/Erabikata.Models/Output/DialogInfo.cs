using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Erabikata.Models.Input;
using Newtonsoft.Json;

namespace Erabikata.Models.Output
{
#nullable enable
    public class DialogInfo
    {
        [JsonProperty(Required = Required.Always)]
        public string Id { get; set; }

        [JsonProperty(Required = Required.Always)]
        public double StartTime { get; }

        [JsonProperty(Required = Required.Always)]
        public WordRef[][] Words { get; }

        public DialogInfo(string id, double startTime, WordRef[][] tokenized)
        {
            StartTime = startTime;
            Words = tokenized;
            Id = id;
        }

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