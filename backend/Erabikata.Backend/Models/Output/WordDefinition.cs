using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Erabikata.Backend.Models.Database;
using Mapster;
using MoreLinq;

namespace Erabikata.Backend.Models.Output
{
    public record WordDefinition(
        int Id,
        IEnumerable<WordDefinition.JapaneseWord> Japanese,
        [AdaptMember(nameof(WordInfo.Meanings))] IEnumerable<WordDefinition.EnglishWord> English,
        WordDefinition.PriorityInfo Priorities
    ) {
        [DataMember]
        public long? GlobalRank { get; set; }

        // ReSharper disable once IdentifierTypo
        public record PriorityInfo(bool News, bool Ichi, bool Spec, bool Freq, bool Gai);

        public record JapaneseWord(string? Kanji, string? Reading);

        public record EnglishWord(IEnumerable<string> Tags, IEnumerable<string> Senses);

        public class MappingConfigRegister : IRegister
        {
            public void Register(TypeAdapterConfig config)
            {
                config.ForType<WordInfo, WordDefinition>()
                    .MapToConstructor(true)
                    .Map(
                        definition => definition.Japanese,
                        info =>
                            info.Kanji.ZipLongest(
                                info.Readings,
                                (kanji, reading) =>
                                    new JapaneseWord(
                                        kanji ?? reading,
                                        kanji != null ? reading : null
                                    )
                            )
                    );

                config.ForType<IReadOnlyCollection<string>, PriorityInfo>()
                    .MapWith(
                        src =>
                            new PriorityInfo(
                                src.Contains("news1"),
                                // ReSharper disable once StringLiteralTypo
                                src.Contains("ichi1"),
                                src.Contains("spec1") || src.Contains("spec2"),
                                src.Contains("nf01") || src.Contains("nf02"),
                                src.Contains("gai1")
                            )
                    );
            }
        }
    }
}
