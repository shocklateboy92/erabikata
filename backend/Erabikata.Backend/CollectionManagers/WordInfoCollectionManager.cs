using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Erabikata.Backend.Models.Actions;
using Erabikata.Backend.Models.Database;
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
                case (DictionaryIngestion ({ } dictionary)):
                    await _mongoCollection.DeleteManyAsync(FilterDefinition<WordInfo>.Empty);
                    await _mongoCollection.InsertManyAsync(
                        dictionary,
                        new InsertManyOptions {IsOrdered = false}
                    );
                    break;
            }
        }

        public Task<List<NormalizedWord>> GetAllNormalizedForms()
        {
            return _mongoCollection.Aggregate()
                .Unwind<WordInfo, NormalizedWord>(word => word.Normalized)
                .Project(doc => new NormalizedWord(doc._id, doc.Normalized))
                .ToListAsync();
        }

        // Mongo is ignoring the [BsonId] attribute for some reason, so I the only
        // way I could get his to work is to name the field `_id`
        // ReSharper disable once InconsistentNaming
        public record NormalizedWord(int _id, IReadOnlyList<string> Normalized)
        {
            public uint Count = 0;
        };

        public Task UpdateWordCounts(IEnumerable<NormalizedWord> words)
        {
            var models = words.Select(
                word => new UpdateOneModel<WordInfo>(
                    new ExpressionFilterDefinition<WordInfo>(w => w.Id == word._id),
                    Builders<WordInfo>.Update.Set(w => w.TotalOccurrences, word.Count)
                )
            ).ToArray();

            return _mongoCollection.BulkWriteAsync(models);
        }

        public async Task<WordInfo?> SearchWord(string baseForm)
        {
            return await _mongoCollection.Find(
                    word => word.Kanji.Contains(baseForm) || word.Readings.Contains(baseForm)
                )
                .FirstOrDefaultAsync();
        }

        public Task<List<WordInfo>> GetWords(IEnumerable<int> ids)
        {
            return _mongoCollection.Find(word => ids.Contains(word.Id)).ToListAsync();
        }
    }
}