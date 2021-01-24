using System.Runtime.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Erabikata.Backend.Controllers
{
    [DataContract]
    public record UserInfo([property: BsonId]string Id)
    {
        [DataMember] public string? TodoistToken { get; init; }
    }
}