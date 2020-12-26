using System;
using System.Collections.Generic;
using System.Linq;
using Erabikata.Backend.Managers;
using Erabikata.Models;
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

        public SubsController(
            SubtitleDatabaseManager database)
        {
            _database = database;
        }

        [HttpGet]
        public ActionResult<NowPlayingInfo> Index(
            [FromQuery] [BindRequired] string episode,
            [FromQuery] double time = 0.0,
            [FromQuery] int count = 3)
        {
            if (int.TryParse(episode, out var episodeId) &&
                _database.AllEpisodesV2.ContainsKey(episodeId))
            {
                var startIndex = Math.Max(
                    0,
                    Array.FindIndex(
                        _database.AllEpisodesV2[episodeId].FilteredInputSentences.ToArray(),
                        sentence => sentence.Time >= time
                    ) - count
                );

                return new NowPlayingInfo(
                    episodeId.ToString(),
                    time,
                    _database.AllEpisodesV2[episodeId].FilteredInputSentences
                        .Skip(startIndex)
                        .Take(count * 2)
                        .Select(
                            sentence => new DialogInfo(
                                sentence,
                                _database.AllEpisodesV2[episodeId].KuromojiAnalyzedSentences[sentence.Index]
                            )
                        )
                );
            }

            var ep = _database.AllEpisodes.SingleOrDefault(ep => ep.Title == episode);
            if (ep == null)
            {
                return NotFound();
            }

            return new NowPlayingInfo(episode, time, GetClosestSubs(time, count, ep));
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