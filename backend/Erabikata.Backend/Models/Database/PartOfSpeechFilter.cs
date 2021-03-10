using MongoDB.Bson.Serialization.Attributes;

namespace Erabikata.Backend.Models.Database
{
    public record PartOfSpeechFilter([property: BsonId] string PartOfSpeech, bool IgnoreReading);
}