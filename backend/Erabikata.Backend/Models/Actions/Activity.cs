using System.Runtime.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using NJsonSchema.Converters;

namespace Erabikata.Backend.Models.Actions
{
    [BsonDiscriminator(DiscriminatorName)]
    [BsonKnownTypes(typeof(LearnWord), typeof(UnlearnWord))]
    [JsonConverter(typeof(JsonInheritanceConverter), DiscriminatorName)]
    [KnownType(typeof(LearnWord))]
    [KnownType(typeof(UnlearnWord))]
    public class Activity
    {
        private const string DiscriminatorName = "activityType";
    }
}