using System.Runtime.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Erabikata.Backend.Models.Database;

[DataContract]
public record UserInfo([property: BsonId] string Id)
{
    [DataMember]
    public string? TodoistToken { get; init; }
}
