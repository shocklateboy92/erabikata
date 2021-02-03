using System.Collections.Generic;
using Erabikata.Backend.Models.Database;
using Mapster;

namespace Erabikata.Backend.Models.Output
{
    public record EngSubsResponse(IEnumerable<EngSubsResponse.Sentence> Dialog)
    {
        public record Sentence(
            string Id,
            double Time,
            [AdaptMember(nameof(EngSub.Lines))]
            IReadOnlyList<string> Text);
    }
}