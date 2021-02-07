using System.Collections;
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
            _mongoCollection =
                mongoDatabase.GetCollection<PartOfSpeechFilter>(nameof(PartOfSpeechFilter));
        }

        public async Task OnActivityExecuting(Activity activity)
        {
            switch (activity)
            {
                case IncludePartOfSpeech({ } partOfSpeech):
                    await _mongoCollection.ReplaceOneAsync(
                        filter => filter.PartOfSpeech == partOfSpeech,
                        new PartOfSpeechFilter(partOfSpeech, Include: true),
                        new ReplaceOptions {IsUpsert = true}
                    );
                    break;
                case ExcludePartOfSpeech({ } partOfSpeech):
                    await _mongoCollection.ReplaceOneAsync(
                        filter => filter.PartOfSpeech == partOfSpeech,
                        new PartOfSpeechFilter(partOfSpeech, Include: false),
                        new ReplaceOptions {IsUpsert = true}
                    );
                    break;
            }
        }

        public Task<List<string>> GetIgnoredPartOfSpeech() =>
            _mongoCollection.Find(filter => !filter.Include)
                .Project(filter => filter.PartOfSpeech)
                .ToListAsync();
    }
}