using System.Runtime.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using NJsonSchema.Converters;

namespace Erabikata.Backend.Models.Actions;

[BsonDiscriminator(DiscriminatorName)]
[BsonKnownTypes(
    typeof(LearnWord),
    typeof(UnlearnWord),
    typeof(LearnReading),
    typeof(UnLearnReading),
    typeof(DictionaryUpdate),
    typeof(IgnoreReadingsOf),
    typeof(IncludeReadingsOf),
    typeof(EnableStyle),
    typeof(DisableStyle),
    typeof(SyncAnki),
    typeof(SendToAnki),
    typeof(BeginIngestion),
    typeof(EndIngestion)
)]
[JsonConverter(typeof(JsonInheritanceConverter), DiscriminatorName)]
[KnownType(typeof(LearnWord))]
[KnownType(typeof(UnlearnWord))]
[KnownType(typeof(LearnReading))]
[KnownType(typeof(UnLearnReading))]
[KnownType(typeof(DictionaryUpdate))]
[KnownType(typeof(IgnoreReadingsOf))]
[KnownType(typeof(IncludeReadingsOf))]
[KnownType(typeof(EnableStyle))]
[KnownType(typeof(DisableStyle))]
[KnownType(typeof(SyncAnki))]
[KnownType(typeof(SendToAnki))]
[KnownType(typeof(BeginIngestion))]
[KnownType(typeof(EndIngestion))]
public record Activity
{
    private const string DiscriminatorName = "activityType";
}
