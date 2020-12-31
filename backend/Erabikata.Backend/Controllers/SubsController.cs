using System;
using System.Collections.Generic;
using System.Linq;
using Erabikata.Backend.Managers;
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

        public SubsController(
            SubtitleDatabaseManager database,
            IOptions<SubtitleProcessingSettings> processingSettings)
        {
            _database = database;
            _processingSettings = processingSettings.Value;
        }

        [HttpGet]
        public ActionResult<NowPlayingInfo> Index(
            [FromQuery] [BindRequired] string episode,
            [FromQuery] double time = 0.0,
            [FromQuery] int count = 3,
            [FromQuery] Analyzer analyzer = Analyzer.Kuromoji)
        {
            if (int.TryParse(episode, out var episodeId) &&
                _database.AllEpisodesV2.ContainsKey(episodeId))
            {
                var episodeV2 = _database.AllEpisodesV2[episodeId];
                var startIndex = Math.Max(
                    0,
                    Array.FindIndex(
                        episodeV2.FilteredInputSentences.ToArray(),
                        sentence => sentence.Time >= time
                    ) - count
                );

                return new NowPlayingInfo(
                    episodeId.ToString(),
                    time,
                    episodeV2.FilteredInputSentences.Skip(startIndex)
                        .Take(count * 2)
                        .Select(
                            sentence => new DialogInfo(
                                sentence,
                                episodeV2.AnalyzedSentences[analyzer][sentence.Index],
                                _processingSettings.IgnoredPartsOfSpeech
                            )
                        )
                ) {EpisodeTitle = $"{episodeV2.Parent.Title} Episode {episodeV2.Number}"};
            }

            var ep = _database.AllEpisodes.FirstOrDefault(e => e.Title == episode);
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