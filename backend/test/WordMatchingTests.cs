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
            // https://erabikata3.apps.lasath.org/ui/dialog?episode=2938&time=450.31&word=2842157&word=2028940&word=1399250&word=1402240&word=1584680&word=1912240
            const string sentence = "どうもこうも　妹がいるとそうなるんだよ";
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
            words[3].InfoIds.Should().Contain(2842157);
            words[5].InfoIds.Should().Contain(1524590);
            words[6].InfoIds.Should().Contain(2028930);
            words[7].InfoIds.Should().Contain(1577980);
            words[8].InfoIds.Should().Contain(1008490);
            words[9].InfoIds.Should().Contain(2137720);
            words[10].InfoIds.Should().Contain(1375610);
            words[11].InfoIds.Should().Contain(2087820);
            words[12].InfoIds.Should().Contain(2087820);
            words[13].InfoIds.Should().Contain(2029090);
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

        [Theory, MemberData(nameof(GetExclusionData))]
        public async Task TestWordMatchingExclusions(
            string text,
            int[] excludedWords)
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
            results.Should().NotContain(excludedWords);
        }

        public static IEnumerable<object[]> GetData() =>
            ReadInputDataFile("wordMatchingInputData.yaml");
        public static IEnumerable<object[]> GetExclusionData() =>
            ReadInputDataFile("wordMatchingExclusionInputData.yaml");

        public static IEnumerable<object[]> ReadInputDataFile(string fileName)
        {
            var deserializer = new DeserializerBuilder().WithNamingConvention(
                    CamelCaseNamingConvention.Instance
                )
                .Build();

            var inputFile = File.ReadAllText(fileName);
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
