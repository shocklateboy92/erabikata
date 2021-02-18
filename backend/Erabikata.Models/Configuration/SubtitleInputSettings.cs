using System.Collections.Generic;
using Erabikata.Models.Input;

namespace Erabikata.Models.Configuration
{
    public class SubtitleProcessingSettings
    {
        public SubtitleProcessingInputSettings Input { get; set; }

        public HashSet<string> IgnoredPartsOfSpeech { get; set; }

        public bool AllowIngestionOfSameCommit { get; set; } = false;
    }
}