using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        public Task<List<WordInfo>> GetAllWords()
        {
            return _mongoCollection.Find(FilterDefinition<WordInfo>.Empty).ToListAsync();
        }

        public async Task<WordInfo?> GetWord(long id) =>
            await _mongoCollection.Find(word => word.Id == id).FirstOrDefaultAsync();

        public async Task<WordInfo?> SearchWord(string baseForm)
        {
            return await _mongoCollection.Find(
                    word => word.Kanji.Contains(baseForm) || word.Readings.Contains(baseForm)
                )
                .FirstOrDefaultAsync();
        }

        public Task<List<WordInfo>> GetWords(int[] ids)
        {
            return _mongoCollection.Find(word => ids.Contains(word.Id)).ToListAsync();
        }
    }
}