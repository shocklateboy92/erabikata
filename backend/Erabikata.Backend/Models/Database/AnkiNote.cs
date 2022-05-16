using MongoDB.Bson.Serialization.Attributes;

namespace Erabikata.Backend.Models.Database;

public record AnkiNote(
    [property: BsonId] long Id,
    int[] WordIds,
    string PrimaryWord,
    string PrimaryWordReading,
    Dialog.Word[] Words
);
