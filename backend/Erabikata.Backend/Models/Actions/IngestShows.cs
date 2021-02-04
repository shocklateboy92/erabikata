using System.Collections.Generic;
using System.Runtime.Serialization;
using Erabikata.Models.Input.V2;

namespace Erabikata.Backend.Models.Actions
{
    public class IngestShows : Activity
    {
        public IngestShows(ICollection<ShowToIngest> showsToIngest)
        {
            ShowsToIngest = showsToIngest;
        }

        [DataMember] public ICollection<ShowToIngest> ShowsToIngest { get; }

        public record ShowToIngest(ICollection<string> Files, ShowInfo Info);
    }
}