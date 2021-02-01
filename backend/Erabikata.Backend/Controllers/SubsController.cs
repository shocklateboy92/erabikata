using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Erabikata.Backend.CollectionManagers;
using Erabikata.Backend.Managers;
using Erabikata.Backend.Models.Database;
using Erabikata.Models;
using Erabikata.Models.Configuration;
using Erabikata.Models.Input;
using Erabikata.Models.Output;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;

namespace Erabikata.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubsController : ControllerBase
    {
        private readonly SubtitleDatabaseManager _database;
        private readonly SubtitleProcessingSettings _processingSettings;
        private readonly DialogCollectionManager _dialogCollection;

        public SubsController(
            SubtitleDatabaseManager database,
            IOptions<SubtitleProcessingSettings> processingSettings,
            DialogCollectionManager dialogCollection)
        {
            _database = database;
            _dialogCollection = dialogCollection;
            _processingSettings = processingSettings.Value;
        }

        [HttpGet]
        public async Task<ActionResult<NowPlayingInfo>> Index(
            [FromQuery] [BindRequired] string episode,
            [FromQuery] Analyzer analyzer,
            [FromQuery] double time = 0.0,
            [FromQuery] int count = 3)
        {
            if (int.TryParse(episode, out var episodeId) &&
                _database.AllEpisodesV2.ContainsKey(episodeId))
            {
                var episodeV2 = _database.AllEpisodesV2[episodeId];
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
                                    list => list.Select(
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
                ) {EpisodeTitle = $"{episodeV2.Parent.Title} Episode {episodeV2.Number}"};
            }

            return BadRequest();
        }

        [HttpPut]
        public async Task<IEnumerable<Dialog>> Insert([FromBody] ICollection<Dialog> dialogs)
        {
            await _dialogCollection.InsertMany(dialogs);
            return dialogs;
        }

        private static IEnumerable<DialogInfo> GetClosestSubs(double time, int count, Episode? ep)
        {
            if (ep == null)
            {
                return Enumerable.Empty<DialogInfo>();
            }

            var min = ep.Dialog.Min(s => Math.Abs(s.StartTime - time));
            var index = Array.FindIndex(ep.Dialog, s => Math.Abs(s.StartTime - time).Equals(min));

            if (index < 0)
            {
                throw new InvalidOperationException(
                    "Something went wrong in my linq query to find the index"
                );
            }

            return ep.Dialog.Skip(index - count)
                .Take(count * 2)
                .Select(sentence => new DialogInfo(sentence));
        }
    }
}