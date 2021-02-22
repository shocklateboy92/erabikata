using System.Collections.Generic;
using System.Runtime.Serialization;
using MongoDB.Bson;

namespace Erabikata.Backend.Models.Database
{
    [DataContract]
    public record EngSub
    {
        public EngSub(
            ObjectId id,
            int episodeId,
            double time,
            string[] lines,
            bool isComment,
            string style)
        {
            Id = id;
            EpisodeId = episodeId;
            Time = time;
            Lines = lines;
            IsComment = isComment;
            Style = style;
        }

        [DataMember] public ObjectId Id { get; set; }

        [DataMember] public int EpisodeId { get; set; }

        [DataMember] public double Time { get; set; }

        [DataMember] public IReadOnlyList<string> Lines { get; set; }

        [DataMember] public bool IsComment { get; set; }

        [DataMember] public string Style { get; set; }
    }
}