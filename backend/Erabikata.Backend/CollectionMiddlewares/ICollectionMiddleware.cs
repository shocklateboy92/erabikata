using System;
using System.Threading.Tasks;
using Erabikata.Backend.Models.Actions;

namespace Erabikata.Backend.CollectionMiddlewares
{
    public interface ICollectionMiddleware
    {
        public Task Execute(Activity activity, Func<Activity, Task> next);
    }
}