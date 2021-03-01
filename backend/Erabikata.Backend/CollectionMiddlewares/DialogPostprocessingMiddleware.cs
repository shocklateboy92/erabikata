using System;
using System.Threading.Tasks;
using Erabikata.Backend.CollectionManagers;
using Erabikata.Backend.Models.Actions;
using TaskTupleAwaiter;

namespace Erabikata.Backend.CollectionMiddlewares
{
    public class DialogPostprocessingMiddleware : ICollectionMiddleware
    {
        private readonly DialogCollectionManager _dialog;
        private readonly WordInfoCollectionManager _wordInfo;

        public DialogPostprocessingMiddleware(
            WordInfoCollectionManager wordInfo,
            DialogCollectionManager dialog)
        {
            _wordInfo = wordInfo;
            _dialog = dialog;
        }

        public async Task Execute(Activity activity, Func<Activity, Task> next)
        {
            await next(activity);
            if (activity is BeginIngestion)
            {
                var (words, readings) = await (_wordInfo.GetAllNormalizedForms(), _wordInfo.GetAllReadings());
                await _dialog.ProcessWords2(words, readings);
                await _wordInfo.UpdateWordCounts(words);
            }
        }
    }
}