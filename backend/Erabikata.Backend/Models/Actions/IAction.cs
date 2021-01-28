using System.Runtime.Serialization;
using JsonSubTypes;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Erabikata.Backend.Models.Actions
{
    [JsonConverter(typeof(JsonSubtypes), nameof(ActivityType))]
    [JsonSubtypes.KnownSubTypeAttribute(typeof(LearnWord), nameof(LearnWord))]
    [JsonSubtypes.KnownSubTypeAttribute(typeof(UnlearnWord), nameof(UnlearnWord))]
    [BsonDiscriminator(nameof(ActivityType))]
    [BsonKnownTypes(typeof(LearnWord), typeof(UnlearnWord))]
    public class Activity
    {
        protected Activity(string activityType)
        {
            ActivityType = activityType;
        }

        [DataMember]
        public string ActivityType { get; set; }
    }
}