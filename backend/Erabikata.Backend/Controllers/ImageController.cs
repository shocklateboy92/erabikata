using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Erabikata.Backend.CollectionManagers;
using Erabikata.Models.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Xabe.FFmpeg;

namespace Erabikata.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly EpisodeInfoCollectionManager _episodeInfoCollectionManager;
        private readonly VideoInputSettings _settings;
        private static readonly ConcurrentDictionary<string, Lazy<Task>> _tasks = new();

        public ImageController(
            IOptions<VideoInputSettings> settings,
            EpisodeInfoCollectionManager episodeInfoCollectionManager
        )
        {
            _episodeInfoCollectionManager = episodeInfoCollectionManager;
            _settings = settings.Value;
        }

        [Route("{episodeId}/{time}")]
        public async Task<ActionResult> Index(int episodeId, double time, bool includeSubs = true)
        {
            var episodeInfo = await _episodeInfoCollectionManager.GetEpisodeInfo(episodeId);
            if (episodeInfo != null)
            {
                var input = episodeInfo.File.Replace("/mnt/data", _settings.RootDirectory);
                var cachePath =
                    $"{_settings.ImageCacheDir}/{includeSubs}-{episodeId}-{time:00000.00}.png";

                await _tasks.GetOrAdd(
                    cachePath,
                    new Lazy<Task>(
                        async () =>
                        {
                            Directory.CreateDirectory(_settings.ImageCacheDir);

                            var conversion = await FFmpeg.Conversions.FromSnippet.Snapshot(
                                input,
                                cachePath,
                                TimeSpan.FromSeconds(time)
                            );

                            if (includeSubs)
                            {
                                // ffmpeg interprets ':' as delimiter in 'subtitles=' argument
                                var subInputFile = input.Replace(":", @"\:");

                                // https://stackoverflow.com/a/59576487
                                conversion.AddParameter("-copyts");

                                if (episodeInfo.SubTracks?.Count > 0)
                                {
                                    var mediaInfo = await FFmpeg.GetMediaInfo(input);
                                    // Turns out the subtitle filter does not want the stream id,
                                    // but rather the index of the stream w.r.t. sub streams
                                    var subIndex = mediaInfo.SubtitleStreams
                                        .ToList()
                                        .FindIndex(
                                            sub => episodeInfo.SubTracks.Contains(sub.Title)
                                        );
                                    if (subIndex > 0)
                                    {
                                        // https://ffmpeg.org/ffmpeg-filters.html#subtitles-1
                                        conversion.AddParameter(
                                            $"-vf subtitles=\"'{subInputFile}'\":si={subIndex}"
                                        );
                                    }
                                }
                                else
                                {
                                    // https://superuser.com/a/1309658
                                    conversion.AddParameter($"-vf subtitles=\"'{subInputFile}'\"");
                                }
                            }

                            await conversion.Start();
                        }
                    )
                ).Value;

                return PhysicalFile(cachePath, "image/png");
            }

            return new NotFoundResult();
        }
    }
}
