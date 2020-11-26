using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Erabikata.Models.Input
{
    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                ConjugationTypeConverter.Singleton,
                PartOfSpeechConverter.Singleton,
                new IsoDateTimeConverter {DateTimeStyles = DateTimeStyles.AssumeUniversal}
            }
        };
    }
}