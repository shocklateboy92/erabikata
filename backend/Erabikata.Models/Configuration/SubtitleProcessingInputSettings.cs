using System.Text.RegularExpressions;

namespace Erabikata.Models.Configuration
{
    public class SubtitleProcessingInputSettings
    {
        public string RootDirectory { get; set; }

        public string DirectoryFilter { get; set; }

        public string FileFilter { get; set; } = "*.json";

        public Regex EpisodeNamePattern { get; set; } = new Regex(@"Episode (\d+)\.json$");

        public string VideoMappingFileName { get; set; } = "video_mapping.txt";
    }
}