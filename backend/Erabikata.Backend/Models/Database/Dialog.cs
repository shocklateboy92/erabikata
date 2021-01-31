using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Erabikata.Backend.Models.Database
{
    public record Dialog
    {
        public Dialog(ObjectId id, string episodeId, double time)
        {
            Id = id;
            EpisodeId = episodeId;
            Time = time;
        }

        [BsonId] [DataMember] public ObjectId Id { get; set; }

        [DataMember] public string EpisodeId { get; }

        [DataMember] public double Time { get; init; }

        [DataMember]
        public IReadOnlyList<IReadOnlyList<Word>> Lines { get; init; } =
            new List<IReadOnlyList<Word>>();

        [DataContract]
        public record Word
        {
            public Word(string baseForm, string dictionaryForm)
            {
                BaseForm = baseForm;
                DictionaryForm = dictionaryForm;
            }

            [DataMember] public string BaseForm { get; set; }

            [DataMember] public string DictionaryForm { get; set; }

            [DataMember] public string? Reading { get; set; }
        }
    }
}