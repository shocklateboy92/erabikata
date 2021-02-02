using System.Runtime.Serialization;
using MongoDB.Bson;

namespace Erabikata.Backend.Models.Database
{
    [DataContract]
    public class EngSub
    {
        [DataMember] public ObjectId Id { get; set; }

        public EngSub(ObjectId id, double time, string[] lines, bool isComment, string style)
        {
            Id = id;
            Time = time;
            Lines = lines;
            IsComment = isComment;
            Style = style;
        }

        [DataMember] public double Time { get; set; }

        [DataMember] public string[] Lines { get; set; }

        [DataMember] public bool IsComment { get; set; }

        [DataMember] public string Style { get; set; }
    }
}