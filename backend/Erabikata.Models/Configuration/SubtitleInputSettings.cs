namespace Erabikata.Models.Configuration
{
    public class SubtitleProcessingSettings
    {
        public SubtitleProcessingInputSettings Input { get; set; }

        public bool AllowIngestionOfSameCommit { get; set; } = false;
    }
}
