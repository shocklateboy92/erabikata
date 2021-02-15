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
        public WordDefinition(string id, IEnumerable<JapaneseWord> japanese)
        {
            Id = id;
            Japanese = japanese;
        }

        [JsonProperty(Required = Required.Always)]
        public string Id { get; }

        [JsonProperty(Required = Required.Always)]
        public IEnumerable<JapaneseWord> Japanese { get; }

        [JsonProperty(Required = Required.Always)]
        public IEnumerable<EnglishWord> English { get; } = Enumerable.Empty<EnglishWord>();

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
                    .Map(
                        definition => definition.Japanese,
                        info => info.Kanji.ZipLongest(
                            info.Readings,
                            (kanji, reading) => new JapaneseWord(kanji, reading)
                        )
                    );
            }
        }
    }
}