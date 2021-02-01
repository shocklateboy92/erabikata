using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Erabikata.Backend.CollectionManagers;
using Erabikata.Backend.Managers;
using Erabikata.Models.Input;
using Erabikata.Models.Output;
using Microsoft.AspNetCore.Mvc;

namespace Erabikata.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WordsController : ControllerBase
    {
        private readonly WordCountsManager _wordCountsManager;
        private readonly WordStateManager _knownWordsProvider;
        private readonly DialogCollectionManager _dialogCollectionManager;

        public WordsController(
            WordCountsManager wordCountsManager,
            WordStateManager knownWordsProvider,
            DialogCollectionManager dialogCollectionManager)
        {
            _wordCountsManager = wordCountsManager;
            _knownWordsProvider = knownWordsProvider;
            _dialogCollectionManager = dialogCollectionManager;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IEnumerable<WordInfo>> Ranked(
            [FromQuery] bool respectPartOfSpeechFilter = true,
            [FromQuery] bool excludeKnownWords = true,
            [FromQuery] int max = 100,
            [FromQuery] int skip = 0,
            [FromQuery] HashSet<string>? onlyPartsOfSpeech = null,
            Analyzer analyzer = Analyzer.Kuromoji)
        {
            IReadOnlyCollection<string>? knownWords = null;
            if (excludeKnownWords)
            {
                knownWords = await _knownWordsProvider.SelectAllKnownWordsMap();
            }


            return _wordCountsManager.WordRanks[analyzer.ToAnalyzerMode()]
                .Select(
                    (wordCount) => new WordInfo
                    {
                        Text = wordCount.BaseForm,
                        TotalOccurrences = wordCount.Count,
                        Rank = _wordCountsManager.WordRanksMap[analyzer.ToAnalyzerMode()][
                            wordCount.BaseForm],
                    }
                )
                .Skip(skip)
                .Take(max);
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
    }
}