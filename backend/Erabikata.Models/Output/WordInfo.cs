using System.Collections.Generic;
using System.Linq;
using Erabikata.Models.Input;
using Newtonsoft.Json;

namespace Erabikata.Models.Output
{
    public class WordInfo
    {
        [JsonProperty(Required = Required.Always)]
        public string Text { get; set; }

        [JsonProperty(Required = Required.Always)] public int Rank { get; set; } = -1;

        [JsonProperty(Required = Required.Always)]
        public int TotalOccurrences { get; set; }

        [JsonProperty(Required = Required.Always)]
        public IEnumerable<Occurence> Occurrences { get; set; } = Enumerable.Empty<Occurence>();

        [JsonProperty(Required = Required.Always)]
        public PagingInfo PagingInfo { get; set; }

        public class Occurence
        {
            [JsonProperty(Required = Required.Always)]
            public string EpisodeName { get; set; }

            [JsonProperty(Required = Required.Always)]
            public double Time { get; set; }

            [JsonProperty(Required = Required.Always)]
            public DialogInfo Text { get; set; }

            [JsonProperty] public string VlcCommand { get; set; }

            [JsonProperty(Required = Required.Always)]
            public string SubsLink { get; set; }

            [JsonProperty(Required = Required.Always)]
            public string EpisodeId { get; set; }
        }
    }
}