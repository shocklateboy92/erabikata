using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Erabikata.Models.Input;
using Erabikata.Models.Input.V2;
using Newtonsoft.Json;

namespace Erabikata.Models.Output
{
#nullable enable
    public class DialogInfo
    {
        [JsonProperty(Required = Required.Always)]
        public double StartTime { get; }

        [JsonProperty(Required = Required.Always)]
        public WordRef[][] Words { get; }

        public DialogInfo(double startTime, WordRef[] tokenized)
        {
            StartTime = startTime;
            Words = new[] {tokenized};
        }

        public DialogInfo(
            InputSentence sentence,
            AnalyzedSentenceV2 analyzedSentenceV2,
            ICollection<string> ignoredPartsOfSpeech)
        {
            StartTime = sentence.Time;
            Words = analyzedSentenceV2.Analyzed.Select(
                    a => a.Select(
                            f => new WordRef(
                                f.Original,
                                f.Base == "*" ? f.Original : f.Base,
                                f.PartOfSpeech.Any(ignoredPartsOfSpeech.Contains)
                                    ? string.Empty
                                    : f.Reading
                            )
                        )
                        .ToArray()
                )
                .ToArray();
        }

        public DialogInfo(Sentence sentence)
        {
            this.StartTime = sentence.StartTime;
            this.Words = new[]
            {
                sentence.Analyzed.Select(
                        word => new WordRef(
                            displayText: word.Original,
                            baseForm: word.Base == "*" ? word.Original : word.Base,
                            reading: word.Reading
                        )
                    )
                    .ToArray()
            };
        }

        public class WordRef
        {
            public WordRef(string displayText, string baseForm, string reading)
            {
                DisplayText = displayText;
                BaseForm = baseForm;
                Reading = string.Join(
                    string.Empty,
                    reading.Select(c => c >= 'ァ' && c <= 'ヴ' ? (char) (c - 0x0060) : c)
                );
            }

            [JsonProperty(Required = Required.Always)]
            public string DisplayText { get; }

            [JsonProperty(Required = Required.Always)]
            public string BaseForm { get; }

            [JsonProperty(Required = Required.Always)]
            public string Reading { get; }
        }
    }
#nullable restore
}