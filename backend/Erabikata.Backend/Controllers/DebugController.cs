using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Erabikata.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DebugController : ControllerBase
    {
        [HttpGet]
        public object Index()
        {
            return new
            {
                Headers = Request.Headers.ToDictionary(kv => kv.Key, kv => kv.Value),
                User.Identity,
                claims = User.Claims.Select(claim => new {claim.Type, claim.Value}),
                ids = User.Identities.Select(identity => new
                {
                    identity.Name
                })
                ,d = User.FindFirst("oid")?.Value
                // User.Identities
            };
        }
    }
}