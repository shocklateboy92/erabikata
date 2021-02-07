using System.Runtime.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using NJsonSchema.Converters;

namespace Erabikata.Backend.Models.Actions
{
    [BsonDiscriminator(DiscriminatorName)]
    [BsonKnownTypes(
        typeof(LearnWord),
        typeof(UnlearnWord),
        typeof(IncludePartOfSpeech),
        typeof(ExcludePartOfSpeech),
        typeof(BeginIngestion),
        typeof(EndIngestion)
    )]
    [JsonConverter(typeof(JsonInheritanceConverter), DiscriminatorName)]
    [KnownType(typeof(LearnWord))]
    [KnownType(typeof(UnlearnWord))]
    [KnownType(typeof(IncludePartOfSpeech))]
    [KnownType(typeof(ExcludePartOfSpeech))]
    [KnownType(typeof(BeginIngestion))]
    [KnownType(typeof(EndIngestion))]
    public record Activity
    {
        private const string DiscriminatorName = "activityType";
    }
}