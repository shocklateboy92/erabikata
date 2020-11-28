using System.Collections.Generic;
using Newtonsoft.Json;

namespace Erabikata.Models.Input.V2
{
    #nullable enable
    public class EpisodeV2
    {
        public EpisodeV2(AnalyzedSentenceV2[] analyzed)
        {
            Analyzed = analyzed;
        }

        [JsonProperty(Required = Required.Always)]
        public AnalyzedSentenceV2[] Analyzed { get; set; }
        

        
    }
        public class AnalyzedSentenceV2 
        {
            public AnalyzedSentenceV2(Analyzed[][] analyzed)
            {
                Analyzed = analyzed;
            }

            [JsonProperty(Required = Required.Always, PropertyName = "analyzed")]
            public Analyzed[][] Analyzed { get; set; }
        }
}