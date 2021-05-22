namespace Erabikata.Backend.Models.Actions
{
    public record SyncAnki : Activity;

    public record SendToAnki(
        string Text,
        string Meaning,
        string Image,
        string PrimaryWord,
        string PrimaryWordReading,
        string PrimaryWordMeaning,
        string Notes,
        string Link
    ) : Activity;
}
