using System;
using System.Threading.Tasks;
using Erabikata.Backend.CollectionManagers;
using Erabikata.Backend.Models.Actions;

namespace Erabikata.Backend.CollectionMiddlewares
{
    public class DialogPostprocessingMiddleware : ICollectionMiddleware
    {
        private readonly WordInfoCollectionManager _wordInfo;
        private readonly DialogCollectionManager _dialog;

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
                var words = await _wordInfo.GetAllNormalizedForms();
                await _dialog.ProcessWords2(words);
                await _wordInfo.UpdateWordCounts(words);
            }
        }
    }
}