using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Erabikata.Backend.DataProviders;
using Erabikata.Backend.Managers;
using Erabikata.Models;
using Erabikata.Models.Configuration;
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
        private readonly EpisodeInfoManager _episodeInfo;
        private readonly PlexInfoProvider _plexInfoProvider;
        private readonly VideoInputSettings _videoInputSettings;

        public SubsController(
            SubtitleDatabaseManager database,
            EpisodeInfoManager episodeInfo,
            PlexInfoProvider plexInfoProvider,
            IOptions<VideoInputSettings> videoInputSettings)
        {
            _database = database;
            this._episodeInfo = episodeInfo;
            _plexInfoProvider = plexInfoProvider;
            _videoInputSettings = videoInputSettings.Value;
        }

        [HttpGet]
        public ActionResult<NowPlayingInfo> Index(
            [FromQuery] [BindRequired] string episode,
            [FromQuery] double time = 0.0,
            [FromQuery] int count = 3)
        {
            if (int.TryParse(episode, out var episodeId) &&
                _database.InputSentences.ContainsKey(episodeId))
            {
                var startIndex = Math.Max(
                    0,
                    Array.FindIndex(
                        _database.FilteredInputSentences[episodeId],
                        sentence => sentence.Time >= time
                    ) - count
                );

                return new NowPlayingInfo(
                    episodeId.ToString(),
                    time,
                    _database.FilteredInputSentences[episodeId]
                        .Skip(startIndex)
                        .Take(count * 2)
                        .Select(
                            sentence => new DialogInfo(
                                sentence,
                                _database.KuoromojiAnalyzedSentenceV2s[episodeId][sentence.Index]
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

        [HttpGet]
        [Route("[action]")]
        public async Task<IEnumerable<NowPlayingInfo>> NowPlaying(
            [FromQuery] [BindRequired] string plexToken,
            [FromQuery] int count = 3)
        {
            return (await _plexInfoProvider.GetCurrentSessions(plexToken)).MediaContainer?.Metadata
                ?.SelectMany(
                    session => session.Media.SelectMany(m => m.Part.Select(p => p.File))
                        .Where(f => !string.IsNullOrWhiteSpace(f))
                        .Select(f => f.Replace("/mnt/data", _videoInputSettings.RootDirectory))
                        .Select(
                            file =>
                            {
                                var (episode, info) = _database.AllEpisodes
                                    .Zip(
                                        _database.AllEpisodes.Select(
                                            e => _episodeInfo.GetEpisodeInfo(e)
                                        )
                                    )
                                    .FirstOrDefault(ep => ep.Second.VideoFile == file);

                                var time = TimeSpan.FromMilliseconds(session.ViewOffset)
                                    .TotalSeconds;

                                return new NowPlayingInfo(
                                    episode?.Title ?? file,
                                    time,
                                    GetClosestSubs(time, count, episode)
                                ) {EpisodeTitle = info.Title};
                            }
                        )
                ) ?? Enumerable.Empty<NowPlayingInfo>();
        }
    }
}