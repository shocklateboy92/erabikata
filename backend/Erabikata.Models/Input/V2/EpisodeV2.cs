using System.Collections.Generic;
using Newtonsoft.Json;

namespace Erabikata.Models.Input.V2
{
#nullable enable

    public class InputSentence
    {
        public InputSentence(string[] text, double time, string style, long? size)
        {
            Text = text;
            Time = time;
            Style = style;
            Size = size;
        }

        [JsonProperty("text")] public string[] Text { get; set; }

        [JsonProperty("time")] public double Time { get; set; }

        [JsonProperty("style")] public string Style { get; set; }

        [JsonProperty("size", NullValueHandling = NullValueHandling.Ignore)]
        public long? Size { get; set; }
    }

    public class AnalyzedSentenceV2
    {
        public AnalyzedSentenceV2(Analyzed[][] analyzed)
        {
            Analyzed = analyzed;
        }

        [JsonProperty(Required = Required.Always, PropertyName = "analyzed")]
        public Analyzed[][] Analyzed { get; set; }
    }
}