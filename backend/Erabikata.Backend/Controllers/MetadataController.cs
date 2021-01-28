using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Erabikata.Backend.Managers;
using Erabikata.Models;
using Erabikata.Models.Input;
using Erabikata.Models.Output;
using Microsoft.AspNetCore.Mvc;
using Octokit;

namespace Erabikata.Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MetadataController : ControllerBase
    {
        private readonly SubtitleDatabaseManager _database;
        private readonly EpisodeInfoManager _episodeInfoManager;

        private readonly WordCountsManager _wordCountsManager;

        public MetadataController(
            SubtitleDatabaseManager database,
            WordCountsManager wordCountsManager,
            EpisodeInfoManager episodeInfoManager)
        {
            _database = database;
            _wordCountsManager = wordCountsManager;
            _episodeInfoManager = episodeInfoManager;
        }

        [HttpGet]
        [Route("[action]")]
        public object Totals()
        {
            return new
            {
                episodes = _database.AllEpisodes.Count,
                lines = _database.AllEpisodes.Aggregate(
                    0,
                    (i, episode) => episode.Dialog.Length + i
                ),
                words = _database.AllEpisodes.Aggregate(
                    0,
                    (i, ep) => ep.Dialog.Aggregate(0, (d, s) => s.Analyzed.Length + d) + i
                ),
                uniqueWords = _wordCountsManager.WordRanks[Analyzer.Kuromoji].Length
            };
        }

        [HttpGet]
        [Route("[action]/{word}")]
        public IActionResult LinearSearch([FromRoute] string word, [FromQuery] int max = 100)
        {
            return Ok(
                _database.AllEpisodes.SelectMany(
                        episode => episode.Dialog.SelectMany(
                            sentence => sentence.Analyzed.Where(w => w.Base == word)
                        )
                    )
                    .Take(max)
                    .ToArray()
            );
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult SurfaceFormStats([FromQuery] int max = 100)
        {
            return Ok(
                new
                {
                    mismatchedSentences = _database.AllEpisodes.SelectMany(
                            ep => ep.Dialog.SelectMany(
                                line => line.Analyzed
                                    .Where(
                                        a => Joined(line.Tokenized) != Joined(
                                            line.Analyzed.Select(a => a.Original)
                                        )
                                    )
                                    .Select(
                                        nm => new
                                        {
                                            tokenized = Joined(line.Tokenized),
                                            analyzed = Joined(
                                                line.Analyzed.Select(a => a.Original)
                                            )
                                        }
                                    )
                            )
                        )
                        .Take(max)
                }
            );
        }

        [HttpGet]
        [Route("[action]")]
        public IEnumerable<EpisodeInfo> Episodes(bool clearCache = false)
        {
            return _database.AllEpisodes.Select(
                e => _episodeInfoManager.GetEpisodeInfo(e, clearCache, Url)
            );
        }

        [HttpGet]
        [Route("[action]")]
        public Episode? EpisodeContent([FromQuery] string name)
        {
            return _database.AllEpisodes.FirstOrDefault(ep => ep.Title == name);
        }

        [HttpGet]
        [Route("[action]")]
        public IEnumerable<string> EpisodeNames()
        {
            return _database.AllEpisodes.Select(ep => ep.Title);
        }

        private static string Joined(IEnumerable<string> enumerable)
        {
            return string.Join(string.Empty, enumerable);
        }
    }
}