using System.Collections.Generic;
using System.Runtime.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Erabikata.Backend.Models.Database;

[DataContract]
public class AnkiWord
{
    public AnkiWord(int wordId, IEnumerable<long> noteIds)
    {
        WordId = wordId;
        NoteIds = noteIds;
    }

    [BsonId]
    public int WordId { get; set; }

    public IEnumerable<long> NoteIds { get; set; }
}
