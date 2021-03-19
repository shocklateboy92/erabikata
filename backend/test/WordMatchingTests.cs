using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Erabikata.Backend;
using Erabikata.Backend.CollectionManagers;
using Erabikata.Backend.Models.Database;
using Erabikata.Backend.Processing;
using FluentAssertions;
using Xunit;

namespace Erabikata.Tests
{
    public class WordMatchingTests : IClassFixture<WordMatchingTests.WordInfoFixture>
    {
        private readonly WordInfoFixture _fixture;
        private readonly AnalyzerService.AnalyzerServiceClient _analyzerServiceClient;

        public class WordInfoFixture
        {
            public WordInfoFixture(WordInfoCollectionManager wordInfoCollectionManager)
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
            var response = await _analyzerServiceClient.AnalyzeTextAsync(
                new AnalyzeRequest {Mode = Constants.DefaultAnalyzerMode, Text = sentence}
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
    }
}