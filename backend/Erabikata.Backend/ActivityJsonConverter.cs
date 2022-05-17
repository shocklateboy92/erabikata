using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Erabikata.Backend.Models.Actions;

namespace Erabikata.Backend;

public class ActivityJsonConverter : JsonConverter<Activity>
{
    public override Activity? Read(
        ref Utf8JsonReader originalReader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        // Deep copy, since this is on the stack.
        // Will allow us to "peek" ahead without moving the original
        var reader = originalReader;
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException(
                $"Expected object of type Activity but found start of {reader.TokenType}"
            );
        }

        reader.Read();
        if (reader.TokenType != JsonTokenType.PropertyName)
        {
            throw new JsonException("Expected object to start with property");
        }

        var propertyName = reader.GetString();
        if (propertyName != "activityType")
        {
            throw new JsonException(
                "Expected `activityType` to be the first property of Activity object"
            );
        }

        reader.Read();
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException("Expected `activityType` to be of type string");
        }

        var activityTypeName = reader.GetString();
        var activityType = Type.GetType(
            // we know this is not null because we checked token type above
            $"{typeof(Activity).Namespace}.{activityTypeName}"
        );
        if (activityType?.IsAssignableTo(typeof(Activity)) == true)
        {
            return (Activity?)JsonSerializer.Deserialize(ref originalReader, activityType);
        }

        throw new JsonException($"Unknown activity of type '{activityTypeName}'");
    }

    public override void Write(Utf8JsonWriter writer, Activity value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType());
    }
}
