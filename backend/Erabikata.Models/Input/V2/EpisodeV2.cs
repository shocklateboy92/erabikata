using System.Collections.Generic;
using Newtonsoft.Json;

namespace Erabikata.Models.Input.V2
{
#nullable enable
    public class EpisodeV2
    {
        public IReadOnlyList<InputSentence> EnglishSentences { get; set; } =
            new List<InputSentence>();

        public string FilePath { get; set; } = string.Empty;

        public int Id { get; set; }

        public ShowInfo Parent { get; set; } = null!; // TODO: replace with init property

        public int Number { get; set; }
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
}