using System;
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
using SubtitlesParser.Classes;
using SubtitlesParser.Classes.Parsers;

namespace Erabikata.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EngSubsController : ControllerBase
    {
        private readonly SubtitleDatabaseManager _subtitleDatabaseManager;

        public EngSubsController(SubtitleDatabaseManager subtitleDatabaseManager)
        {
            _subtitleDatabaseManager = subtitleDatabaseManager;
        }

        public ActionResult<EngSubsResponse> Index(
            string episodeId,
            double timeStamp,
            int count = 3)
        {
            var sentences =
                int.TryParse(episodeId, out var newEpId) &&
                _subtitleDatabaseManager.AllEpisodesV2.ContainsKey(newEpId)
                    ? _subtitleDatabaseManager.AllEpisodesV2[newEpId].EnglishSentences.ToArray()
                    : null;

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
    }
}