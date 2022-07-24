using NJsonSchema.Generation;

namespace Erabikata.Backend;

public class NonNullableAreRequiredSchemaProcessor : ISchemaProcessor
{
    public void Process(SchemaProcessorContext context)
    {
        var schema = context.Schema;
        foreach (var (key, prop) in schema.Properties)
        {
            if (prop.IsNullableRaw != true)
            {
                schema.RequiredProperties.Add(key);
            }
        }

        foreach (var jsonSchema in schema.AllOf)
        {
            Process(
                new SchemaProcessorContext(
                    context.Type,
                    jsonSchema,
                    context.Resolver,
                    context.Generator,
                    context.Settings
                )
            );
        }
    }
}
