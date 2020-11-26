using Newtonsoft.Json;

namespace Erabikata.Models.Input
{
    public class Analyzed
    {
        [JsonProperty("original")] public string Original { get; set; }

        [JsonProperty("base")] public string Base { get; set; }

        [JsonProperty("partOfSpeech")] public PartOfSpeech[] PartOfSpeech { get; set; }

        [JsonProperty("conjugationType")] public ConjugationType ConjugationType { get; set; }

        [JsonProperty("reading")] public string Reading { get; set; }

        [JsonProperty("pronunciation")] public string Pronunciation { get; set; }
    }
}