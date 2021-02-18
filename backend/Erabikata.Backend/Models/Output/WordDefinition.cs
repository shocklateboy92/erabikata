using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Erabikata.Backend.Models.Database;
using Mapster;
using MoreLinq;
using Newtonsoft.Json;

namespace Erabikata.Backend.Models.Output
{
    public class WordDefinition
    {
        public WordDefinition(
            int id,
            IEnumerable<JapaneseWord> japanese,
            [AdaptMember(nameof(WordInfo.Meanings))]
            IEnumerable<EnglishWord> english,
            PriorityInfo priorities)
        {
            Id = id;
            Japanese = japanese;
            English = english;
            Priorities = priorities;
        }

        [JsonProperty(Required = Required.Always)]
        public int Id { get; }

        [JsonProperty(Required = Required.Always)]
        public IEnumerable<JapaneseWord> Japanese { get; }

        [JsonProperty(Required = Required.Always)]

        public IEnumerable<EnglishWord> English { get; }

        // ReSharper disable once IdentifierTypo
        public record PriorityInfo(bool News, bool Ichi, bool Spec, bool Freq, bool Gai);

        [JsonProperty(Required = Required.Always)]
        public PriorityInfo Priorities { get; set; }

        public record JapaneseWord(string? Kanji, string? Reading);

        [JsonObject(ItemRequired = Required.Always)]
        public record EnglishWord(
            [property: JsonProperty(Required = Required.Always)]
            IEnumerable<string> Tags,
            [property: JsonProperty(Required = Required.Always)]
            IEnumerable<string> Senses);

        public class MappingConfigRegister : IRegister
        {
            public void Register(TypeAdapterConfig config)
            {
                config.ForType<WordInfo, WordDefinition>()
                    .MapToConstructor(true)
                    .Map(
                        definition => definition.Japanese,
                        info => info.Kanji.ZipLongest(
                            info.Readings,
                            (kanji, reading) => new JapaneseWord(
                                kanji ?? reading,
                                kanji != null ? reading : null
                            )
                        )
                    );

                config.ForType<IReadOnlyCollection<string>, PriorityInfo>()
                    .MapWith(
                        src => new PriorityInfo(
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