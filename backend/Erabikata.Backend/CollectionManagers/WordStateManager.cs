using System.Threading.Tasks;
using Erabikata.Backend.Models.Actions;
using Erabikata.Backend.Models.Database;
using MongoDB.Driver;

namespace Erabikata.Backend.CollectionManagers
{
    public class WordStateManager : ICollectionManager
    {
        private readonly IMongoCollection<WordState> _collection;

        public WordStateManager(IMongoCollection<WordState> collection)
        {
            _collection = collection;
        }

        public async Task OnActivityExecuting(Activity activity)
        {
            switch (activity)
            {
                case LearnWord toLearn:
                    await _collection.ReplaceOneAsync(
                        state => state.BaseForm == toLearn.BaseForm,
                        new WordState(toLearn.BaseForm) {IsKnown = true},
                        new ReplaceOptions {IsUpsert = true}
                    );
                    break;
                case UnlearnWord toUnlearn:
                    await _collection.ReplaceOneAsync(
                        word => word.BaseForm == toUnlearn.BaseForm,
                        new WordState(toUnlearn.BaseForm) {IsKnown = false},
                        new ReplaceOptions {IsUpsert = true}
                    );
                    break;
            }
        }
    }
}