using System.Collections.Generic;

namespace Erabikata.Backend.Models.Output
{
    public record WordOccurrences(int WordId, IDictionary<string, IEnumerable<string>> DialogIds);
}
