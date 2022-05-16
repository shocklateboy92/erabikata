using NJsonSchema.Generation;

namespace Erabikata.Backend;

public class NonNullableAreRequiredSchemaProcessor : ISchemaProcessor
{
    public void Process(SchemaProcessorContext context)
    {
        var schema = context.Schema;
        foreach (var (key, prop) in schema.ActualProperties)
        {
            if (prop.IsNullableRaw != true)
            {
                schema.RequiredProperties.Add(key);
            }
        }
    }
}
