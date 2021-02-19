using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Erabikata.Backend.Models.Database
{
    public record Dialog
    {
        public Dialog(ObjectId id, int episodeId, int index, double time, string episodeTitle)
        {
            Id = id;
            EpisodeId = episodeId;
            Index = index;
            Time = time;
            EpisodeTitle = episodeTitle;
        }

        [BsonId] [DataMember] public ObjectId Id { get; set; }

        [DataMember] public int EpisodeId { get; set; }

        [DataMember] public int Index { get; set; }

        [DataMember] public string EpisodeTitle { get; set; }

        [DataMember] public double Time { get; set; }

        [DataMember] public IEnumerable<Line> Lines { get; set; } = Array.Empty<Line>();

        [DataContract]
        public record Word(
            string BaseForm,
            string DictionaryForm,
            string OriginalForm,
            string Reading,
            bool IsInParenthesis = false)
        {
            [DataMember]
            public IEnumerable<string> PartOfSpeech { get; set; } = Array.Empty<string>();

            [DataMember] public ICollection<int> InfoIds { get; set; } = new List<int>();
        }

        public record Line(IReadOnlyList<Word> Words);

        [DataMember] public ICollection<int> WordsToRank { get; set; } = new List<int>();
    }
}