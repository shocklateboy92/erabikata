using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Erabikata.Backend.Models.Actions;
using Erabikata.Backend.Models.Database;
using Erabikata.Backend.Processing;
using Mapster;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Erabikata.Backend.CollectionManagers
{
    public class WordInfoCollectionManager : ICollectionManager
    {
        private readonly IMongoCollection<WordInfo> _mongoCollection;

        public WordInfoCollectionManager(IMongoDatabase mongoDatabase)
        {
            _mongoCollection = mongoDatabase.GetCollection<WordInfo>(nameof(WordInfo));
        }

        public async Task OnActivityExecuting(Activity activity)
        {
            switch (activity)
            {
                case (DictionaryIngestion( { } dictionary)):
                    await _mongoCollection.DeleteManyAsync(FilterDefinition<WordInfo>.Empty);
                    await _mongoCollection.InsertManyAsync(
                        dictionary,
                        new InsertManyOptions { IsOrdered = false }
                    );
                    break;
            }
        }

        public Task<List<NormalizedWord>> GetAllNormalizedForms()
        {
            return _mongoCollection.Aggregate()
                .Project(doc => new NormalizedWord(doc.Id, doc.Normalized))
                .ToListAsync();
        }

        public Task<List<WordReading>> GetAllReadings()
        {
            return _mongoCollection.Find(FilterDefinition<WordInfo>.Empty)
                .Project(word => new WordReading(word.Id, word.Readings))
                .ToListAsync();
        }

        public record WordReading([property: BsonId] int Id, IEnumerable<string> Readings);

        public Task UpdateWordCounts(IEnumerable<(int wordId, ulong count)> words)
        {
            var models = words.Select(
                    word =>
                        new UpdateOneModel<WordInfo>(
                            new ExpressionFilterDefinition<WordInfo>(w => w.Id == word.wordId),
                            Builders<WordInfo>.Update.Set(w => w.TotalOccurrences, word.count)
                        )
                )
                .ToArray();

            return _mongoCollection.BulkWriteAsync(
                models,
                new BulkWriteOptions { IsOrdered = false }
            );
        }

        public record NormalizedWord(
            [property: BsonId] int Id,
            IReadOnlyList<IReadOnlyList<string>> Normalized
        ) {
            public uint Count = 0;
        }

        public Task<long> GetTotalWordCount()
        {
            return _mongoCollection.CountDocumentsAsync(word => word.TotalOccurrences > 0);
        }

        public Task<List<WordInfo>> GetWords(IEnumerable<int> ids)
        {
            return _mongoCollection.Find(word => ids.Contains(word.Id)).ToListAsync();
        }

        public Task<List<WordRank>> GetWordRanks(IEnumerable<int> ids)
        {
            return _mongoCollection.Aggregate()
                .Match(word => word.TotalOccurrences > 0)
                .SortByDescending(word => word.TotalOccurrences)
                .Group(word => string.Empty, infos => new { WordId = infos.Select(i => i.Id) })
                .Unwind(
                    group => group.WordId,
                    new AggregateUnwindOptions<WordRank>
                    {
                        IncludeArrayIndex = nameof(WordRank.GlobalRank)
                    }
                )
                .Match(wr => ids.Contains(wr.WordId))
                .ToListAsync();
        }

        public Task<List<WordInfo>> GetSortedWordCounts(
            IEnumerable<string> ignoredPartsOfSpeech,
            IEnumerable<int> wordsToSkip,
            int pagingInfoMax,
            int pagingInfoSkip
        ) {
            return _mongoCollection.Aggregate()
                .Match(
                    word =>
                        word.TotalOccurrences > 0
                        && !word.Meanings.Any(
                            meaning => meaning.Tags.Any(s => ignoredPartsOfSpeech.Contains(s))
                        )
                        && !wordsToSkip.Contains(word.Id)
                )
                .SortByDescending(info => info.TotalOccurrences)
                .Skip(pagingInfoSkip)
                .Limit(pagingInfoMax)
                .ToListAsync();
        }

        public record WordRank([property: AdaptIgnore] object _id, int WordId, int GlobalRank);

        public Task<List<int>> SearchWords(string query) =>
            _mongoCollection.Find(
                    word =>
                        word.Kanji.Any(kanji => kanji.Contains(query))
                        || word.Readings.Any(reading => reading.Contains(query))
                )
                .SortByDescending(word => word.TotalOccurrences)
                .Project(word => word.Id)
                .ToListAsync();

        public async Task<WordMatcher> BuildWordMatcher()
        {
            var words = await _mongoCollection.Find(FilterDefinition<WordInfo>.Empty)
                .Project(
                    word =>
                        new WordMatcher.Candidate(
                            word.Id,
                            word.Normalized,
                            word.Analyzed,
                            word.Kanji
                        )
                )
                .ToListAsync();

            return new WordMatcher(words);
        }
    }
}
