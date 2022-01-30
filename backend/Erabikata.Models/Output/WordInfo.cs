namespace Erabikata.Models.Output
{
    public class WordOccurrence
    {
        public string EpisodeName { get; set; }

        public double Time { get; set; }

        public DialogInfo Text { get; set; }

        public string EpisodeId { get; set; }
    }
}
