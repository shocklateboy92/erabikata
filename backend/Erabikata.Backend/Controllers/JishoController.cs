using System;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace Erabikata.Backend.Controllers
{
    /// <summary>
    /// Used as a pass-through to get around the lack of CORS in jisho.org
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class JishoController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public JishoController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        [Route("{*path}")]
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://jisho.org");
            var response = await client.GetAsync(
                Request.GetEncodedPathAndQuery().Replace("/api/jisho", string.Empty)
            );
            return Content(
                await response.Content.ReadAsStringAsync(),
                new MediaTypeHeaderValue(MediaTypeNames.Application.Json)
            );
        }
    }
}