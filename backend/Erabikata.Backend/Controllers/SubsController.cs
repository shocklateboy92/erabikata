using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Erabikata.Backend.CollectionManagers;
using Erabikata.Backend.Extensions;
using Erabikata.Backend.Managers;
using Erabikata.Backend.Models.Database;
using Erabikata.Models.Input;
using Erabikata.Models.Output;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Erabikata.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubsController : ControllerBase
    {
        private readonly SubtitleDatabaseManager _database;
        private readonly DialogCollectionManager _dialogCollection;

        public SubsController(
            SubtitleDatabaseManager database,
            DialogCollectionManager dialogCollection)
        {
            _database = database;
            _dialogCollection = dialogCollection;
        }

        [HttpGet]
        public async Task<ActionResult<NowPlayingInfo>> Index(
            [FromQuery] Analyzer analyzer,
            [FromQuery] string episode,
            [FromQuery] double time = 0.0,
            [FromQuery] int count = 3)
        {
            if (int.TryParse(episode, out var episodeId))
            {
                var dialog = await _dialogCollection.GetNearestDialog(
                    episodeId,
                    time,
                    count * 2,
                    analyzer.ToAnalyzerMode()
                );

                return new NowPlayingInfo(
                    episodeId.ToString(),
                    time,
                    dialog.Select(
                        d => new DialogInfo(
                            d.Time,
                            d.Lines.Select(
                                    list => list.Words.Select(
                                            word => new DialogInfo.WordRef(
                                                word.OriginalForm,
                                                word.BaseForm,
                                                word.Reading
                                            )
                                        )
                                        .ToArray()
                                )
                                .ToArray()
                        )
                    )
                ) {EpisodeTitle = dialog.FirstOrDefault()?.EpisodeTitle};
            }

            return BadRequest();
        }
    }
}