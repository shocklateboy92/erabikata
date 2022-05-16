using System.Collections.Generic;
using System.Runtime.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Erabikata.Backend.Models.Database;

public class EpisodeInfo
{
    public EpisodeInfo(int id, string file, HashSet<string>? subTracks)
    {
        Id = id;
        File = file;
        SubTracks = subTracks;
    }

    [BsonId]
    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public string File { get; set; }

    [DataMember]
    public HashSet<string>? SubTracks { get; set; }
}
