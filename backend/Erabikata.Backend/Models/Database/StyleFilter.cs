using System.Collections.Generic;
using System.Runtime.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Erabikata.Backend.Models.Database
{
    public class StyleFilter
    {
        public StyleFilter(
            int showId,
            IEnumerable<string> enabledStyles,
            IEnumerable<int> forEpisodes)
        {
            ShowId = showId;
            EnabledStyles = enabledStyles;
            ForEpisodes = forEpisodes;
        }

        [BsonId] public int ShowId { get; set; }

        [DataMember] public IEnumerable<string> EnabledStyles { get; set; }

        [DataMember] public IEnumerable<int> ForEpisodes { get; set; }
    }
}