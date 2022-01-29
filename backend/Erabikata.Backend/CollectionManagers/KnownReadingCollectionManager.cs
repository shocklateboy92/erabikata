using System.Collections.Generic;
using System.Threading.Tasks;
using Erabikata.Backend.Models.Actions;
using Erabikata.Backend.Models.Database;
using MongoDB.Driver;

namespace Erabikata.Backend.CollectionManagers
{
    public class KnownReadingCollectionManager : ICollectionManager
    {
        private readonly IMongoCollection<KnownReading> _mongoCollection;

        public KnownReadingCollectionManager(IMongoDatabase database)
        {
            _mongoCollection = database.GetCollection<KnownReading>(nameof(KnownReading));
        }

        public async Task OnActivityExecuting(Activity activity)
        {
            switch (activity)
            {
                case LearnReading reading:
                    await _mongoCollection.ReplaceOneAsync(
                        r => r.WordId == reading.WordId,
                        new KnownReading(reading.WordId, isKnown: true),
                        new ReplaceOptions { IsUpsert = true }
                    );
                    break;
                case UnLearnReading reading:
                    await _mongoCollection.ReplaceOneAsync(
                        r => r.WordId == reading.WordId,
                        new KnownReading(reading.WordId, isKnown: false)
                    );
                    break;
            }
        }

        public Task<List<int>> GetAllKnownReadings() =>
            _mongoCollection
                .Find(reading => reading.IsKnown)
                .Project(reading => reading.WordId)
                .ToListAsync();
    }
}
