using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Erabikata.Backend.Managers;
using Erabikata.Models;
using Erabikata.Models.Configuration;
using Erabikata.Models.Input;
using Erabikata.Models.Input.V2;
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
        private readonly EpisodeInfoManager _episodeInfoManager;
        private readonly SubtitleDatabaseManager _subtitleDatabaseManager;

        private readonly Dictionary<string, InputSentence[]> _cache =
            new Dictionary<string, InputSentence[]>();

        public EngSubsController(
            EpisodeInfoManager episodeInfoManager,
            SubtitleDatabaseManager subtitleDatabaseManager)
        {
            _episodeInfoManager = episodeInfoManager;
            _subtitleDatabaseManager = subtitleDatabaseManager;
        }

        public async Task<ActionResult<EngSubsResponse>> Index(
            string episodeId,
            double timeStamp,
            int count = 3)
        {
            var sentences =
                int.TryParse(episodeId, out var newEpId) &&
                _subtitleDatabaseManager.EnglishSentences.ContainsKey(newEpId)
                    ? _subtitleDatabaseManager.EnglishSentences[newEpId]
                    : await ReadLegacyEngSubs(episodeId);

            if (sentences == null)
            {
                return NotFound();
            }

            var index = Array.FindIndex(sentences, sentence => sentence.Time > timeStamp);
            return new EngSubsResponse
            {
                Dialog = sentences.Skip(Math.Max(0, index - count))
                    .Take(count * 2)
                    .Select(
                        sentence =>
                            new EnglishSentence {Text = sentence.Text, Time = sentence.Time}
                    )
            };
        }

        private async Task<LegacyEnglishSentence[]?> ReadLegacyEngSubs(string episodeId)
        {
            var episode = _subtitleDatabaseManager.AllEpisodes.FirstOrDefault(
                ep => ep.Title == episodeId
            );
            if (episode != null)
            {
                var episodeInfo = _episodeInfoManager.GetEpisodeInfo(episode);
                if (!string.IsNullOrEmpty(episodeInfo.EngSubs))
                {
                    return JsonConvert
                        .DeserializeObject<IEnumerable<LegacyEnglishSentence>>(
                            await System.IO.File.ReadAllTextAsync(episodeInfo.EngSubs)
                        )
                        .Where(sentence => sentence.Text.FirstOrDefault()?.Any() ?? false)
                        .ToArray();
                }
            }

            return null;
        }
    }
}