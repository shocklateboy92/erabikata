using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Erabikata.Backend.CollectionManagers;
using Erabikata.Backend.CollectionMiddlewares;
using Erabikata.Backend.Extensions;
using Erabikata.Backend.Models.Actions;
using Erabikata.Backend.Models.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Erabikata.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActionsController : ControllerBase
    {
        private readonly IReadOnlyCollection<ICollectionManager> _collectionManagers;
        private readonly IEnumerable<ICollectionMiddleware> _collectionMiddlewares;
        private readonly ILogger<ActionsController> _logger;
        private readonly IMongoCollection<ActivityExecution> _mongo;

        public ActionsController(
            IMongoCollection<ActivityExecution> mongo,
            IEnumerable<ICollectionManager> collectionManagers,
            ILogger<ActionsController> logger,
            IEnumerable<ICollectionMiddleware> collectionMiddlewares
        ) {
            _mongo = mongo;
            _logger = logger;
            _collectionMiddlewares = collectionMiddlewares;
            _collectionManagers = collectionManagers.ToList();
        }

        [HttpPost]
        [Route("execute")]
        public async Task<ActionResult> Execute([FromBody] Activity activity)
        {
            var execution = new ActivityExecution(ObjectId.Empty, activity);
            await _mongo.InsertOneAsync(execution);

            await ExecuteMiddleware(activity, _collectionMiddlewares);

            return Ok(execution.Id);
        }

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        private Task ExecuteMiddleware(
            Activity previousActivity,
            IEnumerable<ICollectionMiddleware> remaining
        ) {
            var current = remaining.FirstOrDefault();
            if (current != null)
            {
                _logger.LogInformationString(
                    $"Executing middleware' {current.GetType().Name}' for '{previousActivity.GetType().Name}' activity"
                );
                var result = current.Execute(
                    previousActivity,
                    modifiedActivity => ExecuteMiddleware(modifiedActivity, remaining.Skip(1))
                );
                _logger.LogInformationString(
                    $"Completed middleware '{current.GetType().Name}' for '{previousActivity.GetType().Name}' activity"
                );
                return result;
            }

            return ExecuteCollectionManagers(previousActivity);
        }

        private async Task ExecuteCollectionManagers(Activity activity)
        {
            foreach (var manager in _collectionManagers)
            {
                _logger.LogInformationString(
                    $"Executing collection manager '{manager.GetType().Name}' for '{activity.GetType().Name}' activity"
                );
                await manager.OnActivityExecuting(activity);
                _logger.LogInformationString(
                    $"Completed collection manager '{manager.GetType().Name}' for '{activity.GetType().Name}' activity"
                );
            }

            _logger.LogInformationString($"Completed all collection managers");
        }

        [HttpGet]
        [Route("list")]
        public Task<List<ActivityExecution>> List()
        {
            return _mongo.Find(FilterDefinition<ActivityExecution>.Empty)
                .SortByDescending(execution => execution.Id)
                .ToListAsync();
        }
    }
}
