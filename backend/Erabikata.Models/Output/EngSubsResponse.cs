using System.Collections.Generic;
using Newtonsoft.Json;

namespace Erabikata.Models.Output
{
    public class EngSubsResponse
    {
        [JsonProperty(Required = Required.Always)]
        public IEnumerable<EnglishSentence> Dialog { get; set; }
    }
}