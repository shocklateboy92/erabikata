using System;
using Namotion.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NJsonSchema.Generation;

namespace Erabikata.Backend.Models
{
    /// <summary>
    ///     Contract resolver that (essentially) adds <code>[JsonProperty(Required = Required.Always]</code>
    /// </summary>
    public class JsonContractResolver : CamelCasePropertyNamesContractResolver
    {
        private readonly JsonSchemaGeneratorSettings _schemaGeneratorSettings;

        public JsonContractResolver(JsonSchemaGeneratorSettings schemaGeneratorSettings)
        {
            _schemaGeneratorSettings = schemaGeneratorSettings;
        }

        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            var contract = base.CreateObjectContract(objectType);
            foreach (var property in contract.Properties)
                if (!property.HasMemberAttribute && property.UnderlyingName != null)
                {
                    var contextualMember = objectType.GetMember(property.UnderlyingName)[0]
                        .ToContextualMember();
                    var description = _schemaGeneratorSettings.ReflectionService.GetDescription(
                        contextualMember,
                        _schemaGeneratorSettings
                    );
                    property.Required =
                        description.IsNullable ? Required.AllowNull : Required.Always;
                }

            return contract;
        }
    }
}