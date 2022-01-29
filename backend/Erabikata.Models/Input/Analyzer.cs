using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Erabikata.Models.Input
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Analyzer
    {
        Kuromoji,
        SudachiA,
        SudachiB,
        SudachiC
    }
}
