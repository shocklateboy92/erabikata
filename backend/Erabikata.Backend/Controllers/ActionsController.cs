using Erabikata.Backend.Models.Actions;
using Microsoft.AspNetCore.Mvc;

namespace Erabikata.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActionsController : ControllerBase
    {
        [HttpPost]
        [Route("execute")]
        public ActionResult Execute([FromBody] Action action)
        {
            return Ok(action?.GetType().FullName);
        }
    }
}