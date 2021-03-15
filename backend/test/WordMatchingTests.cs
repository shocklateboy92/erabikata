using System.Collections.Generic;
using Erabikata.Backend.CollectionManagers;
using Xunit;

namespace Erabikata.Tests
{
    public class WordMatchingTests
        : IClassFixture<WordMatchingTests.WordInfoFixture>
    {
        private readonly WordInfoFixture _fixture;

        public class WordInfoFixture
        {
            public WordInfoFixture(
                WordInfoCollectionManager wordInfoCollectionManager)
            {
                Normalized = wordInfoCollectionManager.GetAllNormalizedForms().Result;
                Readings = wordInfoCollectionManager.GetAllReadings().Result;
            }

            public IReadOnlyList<WordInfoCollectionManager.NormalizedWord> Normalized { get; }
            public IReadOnlyList<WordInfoCollectionManager.WordReading> Readings { get; }
        }

        public WordMatchingTests(WordInfoFixture data)
        {
            _fixture = data;
        }
    }
}
