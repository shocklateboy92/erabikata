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
            IEnumerable<EnglishWord> english)
        {
            Id = id;
            Japanese = japanese;
            English = english;
        }

        [JsonProperty(Required = Required.Always)]
        public int Id { get; }

        [JsonProperty(Required = Required.Always)]
        public IEnumerable<JapaneseWord> Japanese { get; }

        [JsonProperty(Required = Required.Always)]

        public IEnumerable<EnglishWord> English { get; }

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
            }
        }
    }
}