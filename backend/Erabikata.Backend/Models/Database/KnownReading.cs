using System.Runtime.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Erabikata.Backend.Models.Database
{
    [DataContract]
    public class KnownReading
    {
        public KnownReading(int wordId, bool isKnown)
        {
            WordId = wordId;
            IsKnown = isKnown;
        }

        [BsonId]
        public int WordId { get; set; }

        public bool IsKnown { get; set; }
    }
}
