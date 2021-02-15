using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Erabikata.Backend.Models.Database
{
    public record Dialog
    {
        public Dialog(ObjectId id, int episodeId, double time, string episodeTitle)
        {
            Id = id;
            EpisodeId = episodeId;
            Time = time;
            EpisodeTitle = episodeTitle;
        }

        [BsonId] [DataMember] public ObjectId Id { get; set; }

        [DataMember] public int EpisodeId { get; set; }

        [DataMember] public string EpisodeTitle { get; set; }

        [DataMember] public double Time { get; set; }

        [DataMember] public IEnumerable<Line> Lines { get; set; } = Array.Empty<Line>();

        [DataContract]
        public record Word
        {
            public Word(
                string baseForm,
                string dictionaryForm,
                string originalForm,
                string reading,
                bool isInParenthesis = false)
            {
                BaseForm = baseForm;
                DictionaryForm = dictionaryForm;
                OriginalForm = originalForm;
                Reading = reading;
                IsInParenthesis = isInParenthesis;
            }

            [DataMember] public string BaseForm { get; set; }

            [DataMember] public string DictionaryForm { get; set; }

            [DataMember] public string Reading { get; set; }

            [DataMember] public bool IsInParenthesis { get; set; }

            [DataMember]
            public IEnumerable<string> PartOfSpeech { get; set; } = Array.Empty<string>();

            [DataMember] public string OriginalForm { get; set; }

            [DataMember] public ICollection<ObjectId> InfoIds { get; set; } = new List<ObjectId>();
        }

        public record Line(IReadOnlyList<Word> Words);
    }
}