using System.Threading.Tasks;
using Erabikata.Backend.CollectionManagers;
using Erabikata.Backend.Extensions;
using Erabikata.Backend.Models.Output;
using Erabikata.Models.Input;
using Microsoft.AspNetCore.Mvc;

namespace Erabikata.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EpisodeController : ControllerBase
    {
        private readonly DialogCollectionManager _dialog;

        public EpisodeController(DialogCollectionManager dialog)
        {
            _dialog = dialog;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Episode>> Index(Analyzer analyzer, string id)
        {
            if (!int.TryParse(id, out var episodeId))
            {
                return BadRequest($"'{id}' is not a valid episode Id");
            }

            return new Episode(
                id,
                await _dialog.GetEpisodeDialog(analyzer.ToAnalyzerMode(), episodeId)
            );
        }
    }
}