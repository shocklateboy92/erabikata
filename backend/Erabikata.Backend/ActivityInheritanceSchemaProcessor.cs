using System.Linq;
using NJsonSchema.Generation;

namespace Erabikata.Backend;

public class ActivityInheritanceSchemaProcessor : ISchemaProcessor
{
    public void Process(SchemaProcessorContext context)
    {
        var schema = context.Schema;
        if (schema.DiscriminatorObject == null)
        {
            return;
        }

        foreach (var jsonSchema in schema.DiscriminatorObject.Mapping.Values)
        {
            jsonSchema.ActualSchema.AllOf.Remove(jsonSchema.ActualSchema.AllOf.First());
            schema.OneOf.Add(jsonSchema);
        }
    }
}
