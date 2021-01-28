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
        private readonly IMongoCollection<ActivityExecution> _mongo;

        public ActionsController(IMongoCollection<ActivityExecution> mongo)
        {
            _mongo = mongo;
        }

        [HttpPost]
        [Route("execute")]
        public async Task<ActionResult> Execute([FromBody] Activity activity)
        {
            var execution = new ActivityExecution(ObjectId.Empty, activity);
            await _mongo.InsertOneAsync(execution);

            return Ok(execution.Id);
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