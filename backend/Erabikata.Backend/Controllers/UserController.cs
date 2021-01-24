using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using MongoDB.Driver;

namespace Erabikata.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IMongoDatabase _mongoClient;

        public UserController(IMongoDatabase mongoClient)
        {
            _mongoClient = mongoClient;
        }

        [HttpGet]
        [Route("todoistToken")]
        public async Task<ActionResult<string?>> GetTodoistToken()
        {
            using var cursor = await GetCollection()
                .Find(user => user.Id == User.GetObjectId())
                .ToCursorAsync();
            var doc = await cursor.FirstOrDefaultAsync();
            return doc?.TodoistToken;
        }

        [HttpPut]
        [Route("todoistToken")]
        public async Task<ActionResult<string?>> PutTodoistToken([FromBody] string token)
        {
            var userId = User.GetObjectId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Unable to get `oid` claim from auth token");
            }

            await GetCollection()
                .ReplaceOneAsync(
                    user => user.Id == userId,
                    new UserInfo(userId) {TodoistToken = token},
                    new ReplaceOptions {IsUpsert = true}
                );

            return Ok();
        }

        private IMongoCollection<UserInfo> GetCollection()
        {
            return _mongoClient.GetCollection<UserInfo>(nameof(UserInfo));
        }
    }
}