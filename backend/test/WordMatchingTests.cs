using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Erabikata.Backend;
using Erabikata.Backend.CollectionManagers;
using Erabikata.Backend.Models.Database;
using Erabikata.Backend.Processing;
using FluentAssertions;
using Xunit;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Erabikata.Tests
{
    public class WordMatchingTests
        : IClassFixture<WordMatchingTests.WordInfoFixture>
    {
        private readonly WordInfoFixture _fixture;
        private readonly AnalyzerService.AnalyzerServiceClient _analyzerServiceClient;

        public class WordInfoFixture
        {
            public WordInfoFixture(
                WordInfoCollectionManager wordInfoCollectionManager)
            {
                Matcher = wordInfoCollectionManager.BuildWordMatcher().Result;
            }

            public WordMatcher Matcher { get; }
        }

        public WordMatchingTests(
            WordInfoFixture fixture,
            AnalyzerService.AnalyzerServiceClient analyzerServiceClient)
        {
            _fixture = fixture;
            _analyzerServiceClient = analyzerServiceClient;
        }

        [Fact]
        public async Task TestKnownMatch()
        {
            var sentence = "どうもこうも　妹がいるとそうなるんだよ";
            var response =
                await _analyzerServiceClient.AnalyzeTextAsync(
                    new AnalyzeRequest
                    {
                        Mode = Constants.DefaultAnalyzerMode,
                        Text = sentence
                    }
                );

            var words = response.Words.Select(
                    word => new Dialog.Word(
                        word.BaseForm,
                        word.DictionaryForm,
                        word.Original,
                        word.Reading
                    )
                )
                .ToArray();

            _fixture.Matcher.FillMatchesAndGetWords(words);

            words.Should().Contain(word => word.InfoIds.Contains(2842157));
        }

        [Theory, MemberData(nameof(GetData))]
        public async Task TestWordMatching(string text, int[] expectedWords)
        {
            var response =
                await _analyzerServiceClient.AnalyzeTextAsync(
                    new AnalyzeRequest
                    {
                        Mode = Constants.DefaultAnalyzerMode,
                        Text = text
                    }
                );

            var words = response.Words.Select(
                    word => new Dialog.Word(
                        word.BaseForm,
                        word.DictionaryForm,
                        word.Original,
                        word.Reading
                    )
                )
                .ToArray();

            var results = _fixture.Matcher.FillMatchesAndGetWords(words);
            results.Should().Contain(expectedWords);
        }

        public static IEnumerable<object[]> GetData()
        {
            var deserializer = new DeserializerBuilder().WithNamingConvention(
                    CamelCaseNamingConvention.Instance
                )
                .Build();

            var inputFile = File.ReadAllText("wordMatchingInputData.yaml");
            foreach (var input in deserializer.Deserialize<WordMatchingInput[]>(
                inputFile
            ))
            {
                yield return new object[] { input.Text!, input.Words };
            }
        }

        private class WordMatchingInput
        {
            public string? Text { get; set; }
            public int[] Words { get; set; } = Array.Empty<int>();
            public int Episode { get; set; }
            public double Time { get; set; }
        }
    }
}
