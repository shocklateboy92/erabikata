using System.Collections.Generic;
using Newtonsoft.Json;

namespace Erabikata.Models.Input.V2
{
#nullable enable
    public class ShowInfo
    {
        public ShowInfo(
            string title,
            string key,
            IReadOnlyList<IReadOnlyList<EpisodeInfo>> episodes)
        {
            Title = title;
            Key = key;
            Episodes = episodes;
        }

        [JsonProperty(Required = Required.Always)]
        public string Title { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Key { get; set; }

        [JsonProperty(Required = Required.Always)]
        public IReadOnlyList<IReadOnlyList<EpisodeInfo>> Episodes { get; set; }

        public class EpisodeInfo
        {
            public EpisodeInfo(string key, string file)
            {
                Key = key;
                File = file;
            }

            [JsonProperty(Required = Required.Always)]
            public string Key { get; set; }

            [JsonProperty(Required = Required.Always)]
            public string File { get; set; }
        }
    }
}