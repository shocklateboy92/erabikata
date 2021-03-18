using System.Collections.Generic;
using System.Threading.Tasks;
using Erabikata.Backend;
using Erabikata.Backend.CollectionManagers;
using Erabikata.Backend.Processing;
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
            
            
        }
    }
}