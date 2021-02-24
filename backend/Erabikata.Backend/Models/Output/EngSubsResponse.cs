using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Erabikata.Backend.Models.Database;
using Mapster;
using MongoDB.Driver;

namespace Erabikata.Backend.Models.Output
{
    public record EngSubsResponse
    {
        [DataMember] public IEnumerable<Sentence> Dialog { get; init; } = Array.Empty<Sentence>();

        public record Sentence(
            string Id,
            double Time,
            string EpisodeId,
            [AdaptMember(nameof(EngSub.Lines))] IReadOnlyList<string> Text,
            string? EpisodeTitle);
    }

    public record StylesOfResponse(
        int ShowId,
        IEnumerable<AggregateSortByCountResult<string>> AllStyles,
        IEnumerable<string> EnabledStyles);
}