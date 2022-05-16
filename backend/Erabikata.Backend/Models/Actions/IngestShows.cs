using System.Collections.Generic;
using System.Runtime.Serialization;
using Erabikata.Models.Input.V2;

namespace Erabikata.Backend.Models.Actions;

public record IngestShows(ICollection<IngestShows.ShowToIngest> ShowsToIngest) : Activity
{
    public record ShowToIngest(ICollection<string> Files, ShowInfo Info);
}

public record AltShow(string Prefix, ShowInfo Info, ShowInfo Original);

public record IngestAltShows(IReadOnlyCollection<AltShow> AltShows) : Activity;
