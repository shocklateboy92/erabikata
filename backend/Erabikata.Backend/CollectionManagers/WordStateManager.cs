using System.Threading.Tasks;
using Erabikata.Backend.Models.Actions;

namespace Erabikata.Backend.CollectionManagers
{
    public class WordStateManager : ICollectionManager
    {
        public Task OnActivityExecuting(Activity activity)
        {
            return Task.CompletedTask;
        }
    }
}