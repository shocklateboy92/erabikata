using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Erabikata.Backend.DataProviders;
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
        private readonly KnownWordsProvider _knownWordsProvider;

        public WordsController(
            WordCountsManager wordCountsManager,
            KnownWordsProvider knownWordsProvider)
        {
            _wordCountsManager = wordCountsManager;
            _knownWordsProvider = knownWordsProvider;
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
                knownWords = await _knownWordsProvider.GetKnownWords();
            }


            return _wordCountsManager
                .BuildWordCountsSorted(
                    analyzer,
                    respectPartOfSpeechFilter,
                    onlyPartsOfSpeech,
                    knownWords
                )
                .Select(
                    (wordCount) => new WordInfo
                    {
                        Text = wordCount.word,
                        TotalOccurrences = wordCount.count,
                        Rank = _wordCountsManager.WordRanksMap[analyzer][wordCount.word],
                    }
                )
                .Skip(skip)
                .Take(max);
        }
    }
}