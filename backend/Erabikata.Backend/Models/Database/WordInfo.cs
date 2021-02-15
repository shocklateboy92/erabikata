using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Erabikata.Backend.Models.Database
{
    public class WordInfo
    {
        public WordInfo(ObjectId id, IEnumerable<string> kanji, IEnumerable<string> readings)
        {
            Id = id;
            Kanji = kanji;
            Readings = readings;
        }

        [BsonId] [DataMember] public ObjectId Id { get; set; }

        [DataMember] public IEnumerable<string> Kanji { get; set; }

        [DataMember] public IEnumerable<string> Readings { get; set; }

        [DataMember]
        public IReadOnlyList<IReadOnlyList<string>> Normalized { get; set; } =
            new Collection<IReadOnlyList<string>>();
    }
}