using System.Runtime.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Erabikata.Backend.Models.Database
{
    [DataContract]
    public record WordState
    {
        public WordState(string baseForm)
        {
            BaseForm = baseForm;
        }

        [DataMember]
        [BsonId]
        public string BaseForm { get; set; }

        [DataMember]
        public bool IsKnown { get; set; }
    }
}
