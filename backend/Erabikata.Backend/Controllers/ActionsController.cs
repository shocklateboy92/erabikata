using System.Collections.Generic;
using System.Threading.Tasks;
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
        private readonly IMongoDatabase _mongoDatabase;

        public ActionsController(IMongoDatabase mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;
        }

        [HttpPost]
        [Route("execute")]
        public async Task<ActionResult> Execute([FromBody] Activity activity)
        {
            var collection = _mongoDatabase.GetCollection<ActivityExecution>(nameof(ActivityExecution));
            var execution = new ActivityExecution(ObjectId.Empty, activity);
            await collection.InsertOneAsync(execution);
            return Ok(execution.Id);
        }

        [HttpGet]
        [Route("list")]
        public async Task<List<ActivityExecution>> List()
        {
            var collection = _mongoDatabase.GetCollection<ActivityExecution>(nameof(ActivityExecution));
            var cursor = await collection.FindAsync(FilterDefinition<ActivityExecution>.Empty);
            var list =  await cursor.ToListAsync();
            return list;
        }
    }
}