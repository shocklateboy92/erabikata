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
        [Route("{episodeId}")]
        public async Task<ActionResult<Episode>> Index(Analyzer analyzer, string episodeId)
        {
            if (!int.TryParse(episodeId, out var parsedId))
            {
                return BadRequest($"'{episodeId}' is not a valid episode Id");
            }

            return new Episode(
                episodeId,
                await _dialog.GetEpisodeDialog(analyzer.ToAnalyzerMode(), parsedId)
            );
        }
    }
}