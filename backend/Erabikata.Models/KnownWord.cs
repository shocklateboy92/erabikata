using Newtonsoft.Json;

namespace Erabikata.Models
{
#nullable enable
    public class KnownWord
    {
        [JsonProperty(Required = Required.Always)]
        public string Base { get; set; } = string.Empty;

        public bool Known { get; set; } = false;

        public bool Readable { get; set; } = false;
    }
#nullable restore
}