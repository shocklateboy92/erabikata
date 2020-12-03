using Newtonsoft.Json;

namespace Erabikata.Models.Output
{
    public class EnglishSentence
    {
        [JsonProperty(Required = Required.Always)]
        public double Time { get; set; }

        [JsonProperty(Required = Required.Default)]
        public string[] Text { get; set; }
    }
}