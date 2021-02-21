using System.Collections.Generic;

namespace Erabikata.Backend.Models.Output
{
    public record Episode(string Id, IEnumerable<Episode.Entry> Entries)
    {
        public record Entry(double Time, string DialogId);
    }
}