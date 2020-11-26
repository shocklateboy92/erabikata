using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Erabikata.Backend.Managers;
using Erabikata.Models;
using Erabikata.Models.Configuration;
using Erabikata.Models.Output;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SubtitlesParser.Classes;
using SubtitlesParser.Classes.Parsers;

namespace Erabikata.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EngSubsController : ControllerBase
    {
        private EpisodeInfoManager _episodeInfoManager;
        private SubtitleDatabaseManager _subtitleDatabaseManager;

        public EngSubsController(
            EpisodeInfoManager episodeInfoManager,
            SubtitleDatabaseManager subtitleDatabaseManager)
        {
            _episodeInfoManager = episodeInfoManager;
            _subtitleDatabaseManager = subtitleDatabaseManager;
        }

        public async Task<EngSubsResponse> Index(
            string episodeId,
            double timeStamp,
            int count = 3,
            bool allStyles = true)
        {
            var episodeInfo = _episodeInfoManager.GetEpisodeInfo(
                _subtitleDatabaseManager.AllEpisodes.First(ep => ep.Title == episodeId)
            );

            var sentences = JsonConvert
                .DeserializeObject<IEnumerable<EnglishSentence>>(
                    await System.IO.File.ReadAllTextAsync(episodeInfo.EngSubs)
                )
                .Where(sentence => allStyles || !string.IsNullOrWhiteSpace(sentence.Text))
                .ToList();

            var index = sentences.FindIndex(sentence => sentence.Time > timeStamp);

            return new EngSubsResponse
            {
                Dialog = sentences.Skip(Math.Max(0, index - count)).Take(count * 2)
            };
        }
    }
}