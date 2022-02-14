using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Erabikata.Backend.Models.Database;

public enum AlternateIdType
{
    Episode,
    Show
}

public record AlternateId(
    [property: BsonId] int Id,
    int OriginalId,
    string Prefix,
    [property: BsonRepresentation(BsonType.String)] AlternateIdType Type
);
