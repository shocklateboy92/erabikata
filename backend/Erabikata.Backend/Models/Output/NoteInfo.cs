using Erabikata.Models.Output;

namespace Erabikata.Backend.Models.Output;

public record NoteInfo(
    long Id,
    int[] WordIds,
    string PrimaryWord,
    string PrimaryWordReading,
    DialogInfo.WordRef[] Words
);
