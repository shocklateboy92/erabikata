using Erabikata.Backend.Managers;
using Microsoft.AspNetCore.Mvc;

namespace Erabikata.Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MetadataController : ControllerBase
    {
        private readonly WordCountsManager _wordCountsManager;

        public MetadataController(
            WordCountsManager wordCountsManager)
        {
            _wordCountsManager = wordCountsManager;
        }

        [HttpGet]
        [Route("[action]")]
        public object Totals()
        {
            // TODO: Re-implement this
            return new
            {
                // episodes = _database.AllEpisodes.Count,
                // lines = _database.AllEpisodes.Aggregate(
                //     0,
                //     (i, episode) => episode.Dialog.Length + i
                // ),
                // words = _database.AllEpisodes.Aggregate(
                //     0,
                //     (i, ep) => ep.Dialog.Aggregate(0, (d, s) => s.Analyzed.Length + d) + i
                // ),
                // uniqueWords = _wordCountsManager.WordRanks[Analyzer.Kuromoji].Length
            };
        }
    }
}