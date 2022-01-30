using System.Collections.Generic;

namespace Erabikata.Models.Input.V2
{
#nullable enable
    public class ShowInfo
    {
        public ShowInfo(
            string title,
            string key,
            IReadOnlyList<IReadOnlyList<EpisodeInfo>> episodes
        )
        {
            Title = title;
            Key = key;
            Episodes = episodes;
        }

        public string Title { get; set; }

        public string Key { get; set; }

        public IReadOnlyList<IReadOnlyList<EpisodeInfo>> Episodes { get; set; }

        public class EpisodeInfo
        {
            public EpisodeInfo(string key, string file)
            {
                Key = key;
                File = file;
            }

            public string Key { get; set; }

            public string File { get; set; }
        }
    }
}
