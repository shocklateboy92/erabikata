using Newtonsoft.Json;

namespace Erabikata.Models.Output
{
    public class WordOccurrence
    {
        [JsonProperty(Required = Required.Always)]
        public string EpisodeName { get; set; }

        [JsonProperty(Required = Required.Always)]
        public double Time { get; set; }

        [JsonProperty(Required = Required.Always)]
        public DialogInfo Text { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string EpisodeId { get; set; }
    }
}