using System.Runtime.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Erabikata.Backend.Models.Database
{
    [DataContract]
    public class AnkiWord
    {
        public AnkiWord(int wordId)
        {
            WordId = wordId;
        }

        [BsonId]
        public int WordId { get; set; }
    }
}
