using Mapster;

namespace Erabikata.Backend.Models.Actions;

public record SyncAnki : Activity;

public record SendToAnki(
    string Text,
    string Meaning,
    [property: AdaptIgnore] SendToAnki.ImageRequest Image,
    string PrimaryWord,
    string PrimaryWordReading,
    string PrimaryWordMeaning,
    string Notes,
    string Link
) : Activity
{
    public record ImageRequest(string EpisodeId, double Time, bool IncludeSubs = true);
}
