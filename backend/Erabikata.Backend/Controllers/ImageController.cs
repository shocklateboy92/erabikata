using System.IO;
using Erabikata.Backend.Managers;
using Erabikata.Models.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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
        public ActionResult Index(int episodeId, double time)
        {
            if (_subtitleDatabaseManager.EpisodeFilePaths.ContainsKey(episodeId))
            {
                if (!IOFile.Exists(CachePathOf(episodeId, time)))
                {
                    return Ok($"Creating {CachePathOf(episodeId, time)}");
                }

                return PhysicalFile(CachePathOf(episodeId, time), "image/png");
            }

            return new NotFoundResult();
        }

        private string CachePathOf(int episodeId, double time) =>
            $"{_settings.ImageCacheDir}/ss-{episodeId}-{time:00000.00}.png";
    }
}