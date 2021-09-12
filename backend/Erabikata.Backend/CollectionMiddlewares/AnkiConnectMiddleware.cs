using System;
using System.Threading.Tasks;
using Erabikata.Backend.Models;
using Erabikata.Backend.Models.Actions;

namespace Erabikata.Backend.CollectionMiddlewares
{
    public class AnkiConnectMiddleware : ICollectionMiddleware
    {
        private readonly IAnkiSyncClient _ankiSyncClient;

        public AnkiConnectMiddleware(IAnkiSyncClient ankiSyncClient)
        {
            _ankiSyncClient = ankiSyncClient;
        }

        public async Task Execute(Activity activity, Func<Activity, Task> next)
        {
            switch (activity)
            {
                case SendToAnki sendToAnki:
                    var response = await _ankiSyncClient.Execute(new AddNoteAnkiAction(sendToAnki));
                    response.Unwrap();
                    break;
            }

            await next(activity);
        }
    }
}
