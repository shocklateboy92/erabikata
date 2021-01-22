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
        private readonly SubtitleDatabaseManager _database;

        public WordsController(
            WordCountsManager wordCountsManager,
            KnownWordsProvider knownWordsProvider,
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

        [HttpGet]
        [Route("[action]")]
        public object Search(string query, Analyzer analyzer = Analyzer.SudachiC)
        {
            var matches = new Dictionary<string, ICollection<string>>();
            foreach (var episodeV2 in _database.AllEpisodesV2.Values)
            foreach (var sentenceV2 in episodeV2.AnalyzedSentences[analyzer])
            foreach (var line in sentenceV2.Analyzed)
            foreach (var word in line)
            {
                if (word.Dictionary.Contains(query))
                {
                    if (matches.ContainsKey(word.Base))
                    {
                        matches[word.Base].Add(word.Dictionary);
                    }
                    else
                    {
                        matches.Add(word.Base, new HashSet<string> {word.Dictionary});
                    }
                }
            }

            return new {matches};
        }
    }
}