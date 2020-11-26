using System;

namespace Erabikata.Models.Output
{
    public class EpisodeInfo
    {
        public int Number { get; set; }
        public string Title { get; set; }
        public string FileName { get; set; }
        public string PatternFile { get; set; }
        public string Pattern { get; set; }
        public string VideoFile { get; set; }
        public string PatternGlob { get; set; }
        public string SubsLink { get; set; }
        public string? EngSubs { get; set; }
    }
}