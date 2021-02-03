using System.Collections.Generic;
using System.Threading.Tasks;
using Erabikata.Backend.CollectionManagers;
using Erabikata.Backend.Models.Output;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace Erabikata.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EngSubsController : ControllerBase
    {
        private readonly EngSubCollectionManager _engSubCollectionManager;

        public EngSubsController(EngSubCollectionManager engSubCollectionManager)
        {
            _engSubCollectionManager = engSubCollectionManager;
        }

        public async Task<ActionResult<EngSubsResponse>> Index(
            string episodeId,
            double timeStamp,
            int count = 3)
        {
            if (!int.TryParse(episodeId, out var episode))
            {
                return BadRequest();
            }

            var subs = await _engSubCollectionManager.GetNearestSubs(episode, timeStamp, count);
            return new EngSubsResponse(subs.Adapt<IEnumerable<EngSubsResponse.Sentence>>());
        }
    }
}