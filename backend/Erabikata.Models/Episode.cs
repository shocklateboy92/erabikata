using Erabikata.Models.Input;

namespace Erabikata.Models
{
    public class Episode
    {
        public string Title { get; set; }

        public Sentence[] Dialog { get; set; }
    }
}