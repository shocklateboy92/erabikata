using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Erabikata.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DebugController : ControllerBase
    {
        [HttpGet]
        public object Index()
        {
            return new {Headers = Request.Headers.ToDictionary(kv => kv.Key, kv => kv.Value)};
        }
    }
}