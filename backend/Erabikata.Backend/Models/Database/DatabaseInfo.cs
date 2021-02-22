using System.Runtime.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Erabikata.Backend.Models.Database
{
    public record DatabaseInfo
    {
        public DatabaseInfo(string ingestedCommit)
        {
            IngestedCommit = ingestedCommit;
        }

        [DataMember] [BsonId] public string Id => "hardcoded-single-db-info-instance-id";

        [DataMember] public string IngestedCommit { get; set; }

        [DataMember] public string? CurrentDictionary { get; set; }
    }
}