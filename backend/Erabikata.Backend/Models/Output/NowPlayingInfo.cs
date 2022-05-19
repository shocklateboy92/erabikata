using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Erabikata.Backend.Models.Output
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

        public string EpisodeId { get; }

        public double Time { get; }

        public IEnumerable<DialogInfo> Dialog { get; }

        [DataMember]
        public string? EpisodeTitle { get; set; }
    }
#nullable restore
}
