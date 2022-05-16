using System.Threading.Tasks;
using Erabikata.Backend.Models.Actions;

namespace Erabikata.Backend.CollectionManagers;

public interface ICollectionManager
{
    Task OnActivityExecuting(Activity activity);
}
