using System.Collections.Generic;
using System.Threading.Tasks;
using Erabikata.Backend.Models.Actions;
using Erabikata.Backend.Models.Database;
using MongoDB.Driver;

namespace Erabikata.Backend.CollectionManagers
{
    public class PartOfSpeechFilterCollectionManager : ICollectionManager
    {
        private readonly IMongoCollection<PartOfSpeechFilter> _mongoCollection;

        public PartOfSpeechFilterCollectionManager(IMongoDatabase mongoDatabase)
        {
            _mongoCollection = mongoDatabase.GetCollection<PartOfSpeechFilter>(
                nameof(PartOfSpeechFilter)
            );
        }

        public async Task OnActivityExecuting(Activity activity)
        {
            switch (activity)
            {
                case IgnoreReadingsOf
                ( { } partOfSpeech):
                    await _mongoCollection.ReplaceOneAsync(
                        filter => filter.PartOfSpeech == partOfSpeech,
                        new PartOfSpeechFilter(partOfSpeech, true),
                        new ReplaceOptions { IsUpsert = true }
                    );
                    break;
                case IncludeReadingsOf
                ( { } partOfSpeech):
                    await _mongoCollection.ReplaceOneAsync(
                        filter => filter.PartOfSpeech == partOfSpeech,
                        new PartOfSpeechFilter(partOfSpeech, false),
                        new ReplaceOptions { IsUpsert = true }
                    );
                    break;
            }
        }

        public Task<List<string>> GetIgnoredPartOfSpeech()
        {
            return _mongoCollection.Find(filter => filter.IgnoreReading)
                .Project(filter => filter.PartOfSpeech)
                .ToListAsync();
        }
    }
}
