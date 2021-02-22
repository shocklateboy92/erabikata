using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Erabikata.Models.Output
{
#nullable enable
    public class NowPlayingInfo
    {
        public NowPlayingInfo(string episodeId, double time, IEnumerable<DialogInfo> dialog)
        {
            EpisodeId = episodeId;
            Time = time;
            Dialog = dialog;
        }

        [JsonProperty(Required = Required.Always)]
        public string EpisodeId { get; }

        [JsonProperty(Required = Required.Always)]
        public double Time { get; }

        [JsonProperty(Required = Required.Always)]
        public IEnumerable<DialogInfo> Dialog { get; }

        [DataMember] public string? EpisodeTitle { get; set; }
    }
#nullable restore
}