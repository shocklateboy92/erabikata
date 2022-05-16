using System.Collections.Generic;

namespace Erabikata.Backend.Models.Output;

public record Episode(string Id, string Title, IEnumerable<Episode.Entry> Entries)
{
    public record Entry(double Time, string DialogId);
}
