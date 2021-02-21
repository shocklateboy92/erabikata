using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Erabikata.Backend.CollectionManagers;
using Erabikata.Backend.Extensions;
using Erabikata.Backend.Models.Database;
using Erabikata.Models.Input;
using Erabikata.Models.Output;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using WordInfo = Erabikata.Models.Output.WordInfo;

namespace Erabikata.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubsController : ControllerBase
    {
        private readonly DialogCollectionManager _dialogCollection;

        public SubsController(DialogCollectionManager dialogCollection)
        {
            _dialogCollection = dialogCollection;
        }

        [HttpGet]
        public async Task<ActionResult<NowPlayingInfo>> Index(
            [FromQuery] Analyzer analyzer,
            [FromQuery] string episode,
            [FromQuery] double time = 0.0,
            [FromQuery] int count = 3)
        {
            if (!int.TryParse(episode, out var episodeId))
            {
                return BadRequest();
            }

            var dialog = await _dialogCollection.GetNearestDialog(
                episodeId,
                time,
                count,
                analyzer.ToAnalyzerMode()
            );

            return new NowPlayingInfo(
                episodeId.ToString(),
                time,
                dialog.Select(
                    d => new DialogInfo(
                        d.Id.ToString(),
                        d.Time,
                        d.Lines.Select(
                                list => list.Words.Select(
                                        word => new DialogInfo.WordRef(
                                            word.OriginalForm,
                                            word.BaseForm,
                                            word.Reading,
                                            word.InfoIds
                                        )
                                    )
                                    .ToArray()
                            )
                            .ToArray()
                    )
                )
            ) {EpisodeTitle = dialog.FirstOrDefault()?.EpisodeTitle};
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IEnumerable<WordInfo.Occurence>> ById(
            [BindRequired] Analyzer analyzer,
            [FromQuery] string[] dialogId)
        {
            var dialogs = await _dialogCollection.GetByIds(analyzer.ToAnalyzerMode(), dialogId);
            return dialogs.Select(
                dialog => new WordInfo.Occurence
                {
                    EpisodeId = dialog.EpisodeId.ToString(),
                    EpisodeName = dialog.EpisodeTitle,
                    Text = new DialogInfo(
                        dialog.Id.ToString(),
                        dialog.Time,
                        dialog.Lines.Select(
                                list => list.Words.Select(
                                        word => new DialogInfo.WordRef(
                                            word.OriginalForm,
                                            word.BaseForm,
                                            word.Reading,
                                            word.InfoIds
                                        )
                                    )
                                    .ToArray()
                            )
                            .ToArray()
                    ),
                    Time = dialog.Time,
                }
            );
        }
    }
}