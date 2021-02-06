using System;
using System.IO;
using System.Threading.Tasks;
using Erabikata.Backend.CollectionManagers;
using Erabikata.Models.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Xabe.FFmpeg;
using IOFile = System.IO.File;

namespace Erabikata.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly EpisodeInfoCollectionManager _episodeInfoCollectionManager;
        private readonly VideoInputSettings _settings;

        public ImageController(
            IOptions<VideoInputSettings> settings,
            EpisodeInfoCollectionManager episodeInfoCollectionManager)
        {
            _episodeInfoCollectionManager = episodeInfoCollectionManager;
            _settings = settings.Value;
        }

        [Route("{episodeId}/{time}")]
        public async Task<ActionResult> Index(int episodeId, double time, bool includeSubs = true)
        {
            var filePath = await _episodeInfoCollectionManager.GetFilePathOfEpisode(episodeId);
            if (filePath != null)
            {
                var input = filePath.Replace("/mnt/data", _settings.RootDirectory);
                var cachePath =
                    $"{_settings.ImageCacheDir}/{includeSubs}-{episodeId}-{time:00000.00}.png";

                if (!IOFile.Exists(cachePath))
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

                return PhysicalFile(cachePath, "image/png");
            }

            return new NotFoundResult();
        }
    }
}