using Erabikata.Models.Input;

namespace Erabikata.Models.Configuration
{
    public class SubtitleProcessingSettings
    {
        public SubtitleProcessingInputSettings Input { get; set; }

        public PartOfSpeech[] IgnoredPartsOfSpeech { get; set; }
    }
}