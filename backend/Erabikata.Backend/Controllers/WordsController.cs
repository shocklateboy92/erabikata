using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Erabikata.Backend.CollectionManagers;
using Erabikata.Backend.Extensions;
using Erabikata.Backend.Models.Output;
using Erabikata.Models.Input;
using Erabikata.Models.Output;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TaskTupleAwaiter;

namespace Erabikata.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WordsController : ControllerBase
    {
        private readonly DialogCollectionManager _dialogCollectionManager;
        private readonly PartOfSpeechFilterCollectionManager _partOfSpeechFilter;
        private readonly WordInfoCollectionManager _wordInfo;

        public WordsController(
            DialogCollectionManager dialogCollectionManager,
            PartOfSpeechFilterCollectionManager partOfSpeechFilter,
            WordInfoCollectionManager wordInfo)
        {
            _dialogCollectionManager = dialogCollectionManager;
            _partOfSpeechFilter = partOfSpeechFilter;
            _wordInfo = wordInfo;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IEnumerable<WordInfo>> Ranked(
            [Required] Analyzer analyzer,
            [FromQuery] int max = 100,
            [FromQuery] int skip = 0)
        {
            var ignoredPartsOfSpeech = await _partOfSpeechFilter.GetIgnoredPartOfSpeech();
            var ranks = await _dialogCollectionManager.GetSortedWordCounts(
                analyzer.ToAnalyzerMode(),
                ignoredPartsOfSpeech,
                max,
                skip
            );

            return ranks.Select(
                (result, index) => new WordInfo
                {
                    Text = result.Id, Rank = skip + index, TotalOccurrences = result.Count
                }
            );
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<object> Search(string query, Analyzer analyzer = Analyzer.SudachiC)
        {
            var matchingDialog =
                await _dialogCollectionManager.GetFuzzyMatches(query, analyzer.ToAnalyzerMode());
            var matches = matchingDialog.SelectMany(sentenceV2 => sentenceV2.Lines)
                .SelectMany(
                    line => line.Words.Where(
                        analyzed => analyzed.DictionaryForm.Contains(query) ||
                                    analyzed.BaseForm.Contains(query)
                    )
                )
                .GroupBy(analyzed => analyzed.BaseForm)
                .Select(
                    group => new
                    {
                        baseForm = group.Key,
                        link =
                            $"{Request.Scheme}://{Request.Host}/word/{group.Key}?word={group.Key}",
                        dictionaryForms = group.Select(analyzed => analyzed.DictionaryForm)
                            .Distinct(),
                    }
                );

            return new {matches};
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IEnumerable<WordDefinition>> Definition(
            [BindRequired] [FromQuery] int[] ids)
        {
            var words = await _wordInfo.GetWords(ids);
            return words.Adapt<IEnumerable<WordDefinition>>();
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<Dictionary<int, long?>> EpisodeRank(
            [BindRequired] Analyzer analyzer,
            [BindRequired] int episodeId,
            [BindRequired] [FromQuery] int[] wordId)
        {
            var analyzerMode = analyzer.ToAnalyzerMode();
            var (ranks, total) = await (
                _dialogCollectionManager.GetWordRanks(analyzerMode, episodeId, wordId),
                _dialogCollectionManager.GetEpisodeWordCount(analyzerMode, episodeId));

            return wordId.ToDictionary(
                id => id,
                id => ranks.FirstOrDefault(rank => rank.counts._id == id)?.rank * 100 / total.Count
            );
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<Dictionary<int, long?>> GlobalRank(
            [BindRequired] [FromQuery] int[] wordId)
        {
            var (ranks, total) =
                await (_wordInfo.GetWordRanks(wordId), _wordInfo.GetTotalWordCount());
            return wordId.ToDictionary(
                id => id,
                id => ranks.FirstOrDefault(wr => wr.wordId == id)?.rank * 100 / total
            );
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<object> EpisodeTotal(
            [BindRequired] Analyzer analyzer,
            [BindRequired] int episodeId)
        {
            var results = await _dialogCollectionManager.GetEpisodeWordCount(
                analyzer.ToAnalyzerMode(),
                episodeId
            );

            return results;
        }
    }
}