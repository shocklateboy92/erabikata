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
        private readonly SubtitleDatabaseManager _database;

        public WordsController(
            WordCountsManager wordCountsManager,
            WordStateManager knownWordsProvider,
            SubtitleDatabaseManager database)
        {
            _wordCountsManager = wordCountsManager;
            _knownWordsProvider = knownWordsProvider;
            _database = database;
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

        [HttpGet]
        [Route("[action]")]
        public object Search(string query, Analyzer analyzer = Analyzer.SudachiC)
        {
            var matches = _database.AllEpisodesV2.Values
                .SelectMany(
                    v2 => v2.AnalyzedSentences[analyzer]
                        .SelectMany(sentenceV2 => sentenceV2.Analyzed)
                        .SelectMany(
                            line => line.Where(analyzed => analyzed.Dictionary.Contains(query))
                        )
                )
                .GroupBy(analyzed => analyzed.Base)
                .Select(
                    group => new
                    {
                        baseForm = group.Key,
                        link =
                            $"{Request.Scheme}://{Request.Host}/word/{group.Key}?word={group.Key}",
                        dictionaryForms =
                            group.Select(analyzed => analyzed.Dictionary).Distinct(),
                    }
                );

            return new {matches};
        }
    }
}