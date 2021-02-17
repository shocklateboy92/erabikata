using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Erabikata.Backend.Models.Database
{
    public class WordInfo
    {
        public WordInfo(
            int id,
            IEnumerable<string> kanji,
            IEnumerable<string> readings,
            IEnumerable<Meaning> meanings)
        {
            Id = id;
            Kanji = kanji;
            Readings = readings;
            Meanings = meanings;
        }

        [BsonId] [DataMember] public int Id { get; set; }

        [DataMember] public IEnumerable<string> Kanji { get; set; }

        [DataMember] public IEnumerable<string> Readings { get; set; }

        [DataMember]
        public IReadOnlyList<IReadOnlyList<string>> Normalized { get; set; } =
            new Collection<IReadOnlyList<string>>();

        [DataMember] public IEnumerable<Meaning> Meanings { get; set; }

        public record Meaning(IReadOnlyCollection<string> Tags, IEnumerable<string> Senses);
    }
}