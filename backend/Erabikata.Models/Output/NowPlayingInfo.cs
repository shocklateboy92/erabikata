using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Erabikata.Models.Output
{
#nullable enable
    public class NowPlayingInfo
    {
        [JsonProperty(Required = Required.Always)]
        public string EpisodeId { get; }

        [JsonProperty(Required = Required.Always)]
        public double Time { get; }

        [JsonProperty(Required = Required.Always)]
        public IEnumerable<DialogInfo> Dialog { get; }

        [DataMember]
        public string? EpisodeTitle { get; set; }

        public NowPlayingInfo(string episodeId, double time, IEnumerable<DialogInfo> dialog)
        {
            this.EpisodeId = episodeId;
            this.Time = time;
            this.Dialog = dialog;
        }
    }
#nullable restore
}