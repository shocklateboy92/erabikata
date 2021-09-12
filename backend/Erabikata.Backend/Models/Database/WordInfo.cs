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
            IEnumerable<Meaning> meanings,
            HashSet<string> priorities
        ) {
            Id = id;
            Kanji = kanji;
            Readings = readings;
            Meanings = meanings;
            Priorities = priorities;
        }

        [BsonId]
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public IEnumerable<string> Kanji { get; set; }

        [DataMember]
        public IEnumerable<string> Readings { get; set; }

        [DataMember]
        public IReadOnlyList<IReadOnlyList<string>> Normalized { get; set; } =
            new List<IReadOnlyList<string>>();

        [DataMember]
        public IReadOnlyList<IReadOnlyList<string>> Analyzed { get; set; } =
            new List<IReadOnlyList<string>>();

        [DataMember]
        public IEnumerable<Meaning> Meanings { get; set; }

        [DataMember]
        public IReadOnlyCollection<string> Priorities { get; set; }

        [DataMember]
        public uint TotalOccurrences { get; set; }

        public record Meaning(IReadOnlyCollection<string> Tags, IEnumerable<string> Senses);
    }
}
