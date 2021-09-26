using System;
using System.Collections.Concurrent;
using System.IO;
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
        ) {
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
                                conversion
                                // https://stackoverflow.com/a/59576487
                                .AddParameter("-copyts")
                                    // https://superuser.com/a/1309658
                                    .AddParameter($"-vf subtitles=\"'{input}'\"");
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
