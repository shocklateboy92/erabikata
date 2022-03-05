using System.Collections.Generic;
using System.Linq;
using NJsonSchema;
using NJsonSchema.Generation;

namespace Erabikata.Backend;

public class ActivityInheritanceSchemaProcessor : ISchemaProcessor
{
    public void Process(SchemaProcessorContext context)
    {
        var schema = context.Schema;
        foreach (var jsonSchema in schema.DiscriminatorObject?.Mapping.Values ?? new List<JsonSchema>())
        {
            jsonSchema.ActualSchema.AllOf.Remove(jsonSchema.ActualSchema.AllOf.First());
            schema.OneOf.Add(jsonSchema);
        }
    }
}
