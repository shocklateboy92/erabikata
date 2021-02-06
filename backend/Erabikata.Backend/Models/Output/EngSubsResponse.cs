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
        public class Sentence
        {
            public Sentence(
                string id,
                double time,
                [AdaptMember(nameof(EngSub.Lines))] IReadOnlyList<string> text)
            {
                Id = id;
                Time = time;
                Text = text;
            }

            [DataMember] [Required] public string Id { get; }

            [DataMember] [Required] public double Time { get; }

            [DataMember]
            [Required]
            [AdaptMember(nameof(EngSub.Lines))]
            public IReadOnlyList<string> Text { get; }
        }
    }
}