using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Erabikata.Backend.Models.Database;
using Mapster;

namespace Erabikata.Backend.Models.Output
{
    public record EngSubsResponse
    {
        [Required]
        [DataMember]
        public IEnumerable<Sentence> Dialog { get; init; } = System.Array.Empty<Sentence>();

        [DataContract]
        public record Sentence(
            string Id,
            double Time,
            [AdaptMember(nameof(EngSub.Lines))] IReadOnlyList<string> Text);
    }
}