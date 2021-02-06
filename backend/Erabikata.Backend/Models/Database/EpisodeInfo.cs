using System.Runtime.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Erabikata.Backend.Models.Database
{
    public class EpisodeInfo
    {
        public EpisodeInfo(int id, string file)
        {
            Id = id;
            File = file;
        }

        [BsonId] [DataMember] public int Id { get; set; }

        [DataMember] public string File { get; set; }
    }
}