using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Erabikata.Backend.CollectionManagers;
using Erabikata.Backend.Managers;
using Erabikata.Models.Input;
using Erabikata.Models.Output;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TaskTupleAwaiter;

namespace Erabikata.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WordController : ControllerBase
    {
        private readonly WordCountsManager _wordCounts;
        private readonly DialogCollectionManager _dialogCollectionManager;

        public WordController(
            WordCountsManager wordCounts,
            DialogCollectionManager dialogCollectionManager)
        {
            _wordCounts = wordCounts;
            _dialogCollectionManager = dialogCollectionManager;
        }

        [HttpGet]
        [Route("{text}")]
        public async Task<WordInfo> Index(
            [FromRoute] string text,
            [FromQuery] HashSet<string> onlyPartsOfSpeech,
            string? includeEpisode,
            double? includeTime,
            [FromQuery] PagingInfo pagingInfo,
            [BindRequired] Analyzer analyzer)
        {
            var rank = _wordCounts.WordRanksMap[analyzer.ToAnalyzerMode()]
                .GetValueOrDefault(text, -1);
            if (rank < 0)
            {
                return new WordInfo
                {
                    Text = text,
                    TotalOccurrences = 0,
                    Occurrences = System.Array.Empty<WordInfo.Occurence>()
                };
            }

            var (matches, count) = await (
                _dialogCollectionManager.GetMatches(
                    text,
                    analyzer.ToAnalyzerMode(),
                    pagingInfo.Skip,
                    pagingInfo.Max
                ), _dialogCollectionManager.CountMatches(text, analyzer.ToAnalyzerMode()));
            var occurrences = matches.Select(
                dialog => new WordInfo.Occurence
                {
                    EpisodeId = dialog.EpisodeId.ToString(),
                    EpisodeName = $"tbd",
                    Text = new DialogInfo(
                        dialog.Time,
                        dialog.Lines.Select(
                                list => list.Words.Select(
                                        word => new DialogInfo.WordRef(
                                            word.OriginalForm,
                                            word.BaseForm,
                                            word.Reading
                                        )
                                    )
                                    .ToArray()
                            )
                            .ToArray()
                    ),
                    Time = dialog.Time,
                }
            );

            return new WordInfo
            {
                Text = text,
                Rank = rank,
                TotalOccurrences = count,
                Occurrences = occurrences,
                PagingInfo = pagingInfo
            };
        }

        [HttpGet]
        [Route("{word}/[action]")]
        public async Task<IEnumerable<string>> PartsOfSpeech(
            [FromRoute] string word,
            [Required] Analyzer analyzer)
        {
            var matches = await _dialogCollectionManager.GetMatches(
                word,
                analyzer.ToAnalyzerMode(),
                skip: 0,
                take: int.MaxValue
            );

            return matches.SelectMany(
                    dialog => dialog.Lines.SelectMany(
                        line => line.Words.Where(analyzedWord => analyzedWord.BaseForm == word)
                            .SelectMany(analyzed => analyzed.PartOfSpeech)
                    )
                )
                .Distinct();
        }
    }
}