using System.Threading.Tasks;
using Erabikata.Backend.Models.Actions;
using Microsoft.Extensions.Logging;

namespace Erabikata.Backend.CollectionManagers
{
    public class DummyCollectionManager : ICollectionManager
    {
        private readonly ILogger<DummyCollectionManager> _logger;

        public DummyCollectionManager(ILogger<DummyCollectionManager> logger)
        {
            _logger = logger;
        }

        public Task OnActivityExecuting(Activity activity)
        {
            _logger.LogInformation("Dummy collection manager being dumb");
            return Task.CompletedTask;
        }
    }
}
