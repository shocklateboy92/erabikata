using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Erabikata.Backend.CollectionManagers;
using Erabikata.Backend.CollectionMiddlewares;
using Erabikata.Backend.Models.Actions;
using Erabikata.Backend.Models.Database;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Erabikata.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActionsController : ControllerBase
    {
        private readonly IMongoCollection<ActivityExecution> _mongo;
        private readonly IReadOnlyCollection<ICollectionManager> _collectionManagers;
        private readonly IEnumerable<ICollectionMiddleware> _collectionMiddlewares;

        public ActionsController(
            IMongoCollection<ActivityExecution> mongo,
            IEnumerable<ICollectionManager> collectionManagers,
            IEnumerable<ICollectionMiddleware> collectionMiddlewares)
        {
            _mongo = mongo;
            _collectionMiddlewares = collectionMiddlewares;
            _collectionManagers = collectionManagers.ToList();
        }

        [HttpPost]
        [Route("execute")]
        public async Task<ActionResult> Execute([FromBody] Activity activity)
        {
            var execution = new ActivityExecution(ObjectId.Empty, activity);
            await _mongo.InsertOneAsync(execution);

            var middleware = _collectionMiddlewares.GetEnumerator();

            Task ExecuteMiddleware(Activity previousActivity)
            {
                if (middleware.MoveNext())
                {
                    return middleware.Current.Execute(activity, ExecuteMiddleware);
                }

                middleware.Dispose();
                return ExecuteCollectionManagers(previousActivity);
            }

            await ExecuteMiddleware(activity);

            return Ok(execution.Id);
        }

        private async Task ExecuteCollectionManagers(Activity activity)
        {
            foreach (var manager in _collectionManagers)
            {
                await manager.OnActivityExecuting(activity);
            }
        }

        [HttpGet]
        [Route("list")]
        public async Task<List<ActivityExecution>> List()
        {
            using var cursor = await _mongo.FindAsync(FilterDefinition<ActivityExecution>.Empty);
            return await cursor.ToListAsync();
        }
    }
}