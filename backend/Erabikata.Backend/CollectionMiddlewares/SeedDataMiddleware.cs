using System;
using System.Threading.Tasks;
using Erabikata.Backend.Models.Actions;

namespace Erabikata.Backend.CollectionMiddlewares
{
    public class SeedDataMiddleware : ICollectionMiddleware
    {
        public async Task Execute(Activity activity, Func<Activity, Task> next)
        {
            Console.WriteLine("Before stuff");
            await next(activity);
            Console.WriteLine("After stuff");
        }
    }
}