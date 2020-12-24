using System;
using System.IO;
using System.Threading.Tasks;
using Erabikata.Backend.Managers;
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
        private readonly SubtitleDatabaseManager _subtitleDatabaseManager;
        private readonly VideoInputSettings _settings;

        public ImageController(
            SubtitleDatabaseManager subtitleDatabaseManager,
            IOptions<VideoInputSettings> settings)
        {
            _subtitleDatabaseManager = subtitleDatabaseManager;
            _settings = settings.Value;
        }

        [Route("{episodeId}/{time}")]
        public async Task<ActionResult> Index(int episodeId, double time)
        {
            if (_subtitleDatabaseManager.EpisodeFilePaths.ContainsKey(episodeId))
            {
                if (!IOFile.Exists(CachePathOf(episodeId, time)))
                {
                    var conversion = await FFmpeg.Conversions.FromSnippet.Snapshot(
                        _subtitleDatabaseManager.EpisodeFilePaths[episodeId]
                            .Replace("/mnt/data", _settings.RootDirectory),
                        CachePathOf(episodeId, time),
                        TimeSpan.FromSeconds(time)
                    );

                    await conversion.Start();
                }

                return PhysicalFile(CachePathOf(episodeId, time), "image/png");
            }

            return new NotFoundResult();
        }

        private string CachePathOf(int episodeId, double time) =>
            $"{_settings.ImageCacheDir}/ss-{episodeId}-{time:00000.00}.png";
    }
}