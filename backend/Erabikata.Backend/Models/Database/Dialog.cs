using System;
using System.Runtime.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Erabikata.Backend.Models.Database
{
    public record Dialog
    {
        public Dialog((string episode, double time) id)
        {
            Id = id;
        }

        [BsonId] [DataMember] public (string episode, double time) Id { get; set; }

        public Word[][] Lines { get; } = Array.Empty<Word[]>();

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