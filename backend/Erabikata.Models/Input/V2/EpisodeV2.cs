using System.Collections.Generic;
using Newtonsoft.Json;

namespace Erabikata.Models.Input.V2
{
#nullable enable
    public class EpisodeV2
    {
        public IReadOnlyList<InputSentence> InputSentences { get; set; } =
            new List<InputSentence>();

        public IReadOnlyList<InputSentence> EnglishSentences { get; set; } =
            new List<InputSentence>();

        public string FilePath { get; set; } = string.Empty;

        public IReadOnlyList<FilteredInputSentence> FilteredInputSentences { get; set; } =
            new List<FilteredInputSentence>();

        public int Id { get; set; }

        public ShowInfo Parent { get; set; } = null!; // TODO: replace with init property

        public int Number { get; set; }

        public IReadOnlyDictionary<Analyzer, AnalyzedSentenceV2[]> AnalyzedSentences { get; set; } =
            new Dictionary<Analyzer, AnalyzedSentenceV2[]>();
    }

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

    public class FilteredInputSentence : InputSentence
    {
        public FilteredInputSentence(InputSentence sentence, int index) : base(
            sentence.Text,
            sentence.Time,
            sentence.Style,
            sentence.Size
        )
        {
            Index = index;
        }

        public int Index { get; }
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