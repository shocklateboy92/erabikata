using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Erabikata.Backend.CollectionManagers;
using Erabikata.Backend.Extensions;
using Erabikata.Backend.Models.Output;
using Erabikata.Models.Input;
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
        private readonly AnkiWordCollectionManager _ankiWords;
        private readonly KnownReadingCollectionManager _knownReadings;

        public WordsController(
            DialogCollectionManager dialogCollectionManager,
            PartOfSpeechFilterCollectionManager partOfSpeechFilter,
            WordInfoCollectionManager wordInfo,
            AnkiWordCollectionManager ankiWords,
            KnownReadingCollectionManager knownReadings
        ) {
            _dialogCollectionManager = dialogCollectionManager;
            _partOfSpeechFilter = partOfSpeechFilter;
            _wordInfo = wordInfo;
            _ankiWords = ankiWords;
            _knownReadings = knownReadings;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IEnumerable<WordRankInfo>> Ranked2(
            Analyzer analyzer,
            [FromQuery] PagingInfo pagingInfo,
            bool skipKnown = true
        ) {
            if (analyzer.ToAnalyzerMode() != Constants.DefaultAnalyzerMode)
            {
                return Array.Empty<WordRankInfo>();
            }

            var wordsToSkip = skipKnown ? await _ankiWords.GetAllKnownWords() : new();
            var ranks = await _wordInfo.GetSortedWordCounts(
                ArraySegment<string>.Empty,
                wordsToSkip,
                pagingInfo.Max,
                pagingInfo.Skip
            );

            return ranks.Select(
                (word, index) =>
                    new WordRankInfo(
                        word.Id,
                        index + pagingInfo.Skip,
                        word.TotalOccurrences,
                        word.Kanji.FirstOrDefault() ?? word.Readings.First()
                    )
            );
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IEnumerable<int>> Search(string query)
        {
            return await _wordInfo.SearchWords(query);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IEnumerable<WordDefinition>> Definition(
            [BindRequired] [FromQuery] int[] wordId
        ) {
            var (infos, ranks, totalCount) = await (
                _wordInfo.GetWords(wordId),
                _wordInfo.GetWordRanks(wordId),
                _wordInfo.GetTotalWordCount()
            );
            var definitions = infos.Adapt<List<WordDefinition>>();
            foreach (var definition in definitions)
                definition.GlobalRank =
                    ranks.FirstOrDefault(wr => wr.WordId == definition.Id)?.GlobalRank * 100
                        / totalCount
                    + 1;

            return definitions;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<IEnumerable<WordRank>>> EpisodeRank(
            [BindRequired] Analyzer analyzer,
            [BindRequired] string episodeId,
            [BindRequired] [FromQuery] int[] wordId
        ) {
            if (!int.TryParse(episodeId, out var episode))
            {
                return BadRequest($"{nameof(episodeId)} must be a number");
            }

            var analyzerMode = analyzer.ToAnalyzerMode();
            var (ranks, total) = await (
                _dialogCollectionManager.GetWordRanks(analyzerMode, episode, wordId),
                _dialogCollectionManager.GetEpisodeWordCount(analyzerMode, episode)
            );

            return Ok(
                wordId.Select(
                    id =>
                        new WordRank(
                            id,
                            ranks.FirstOrDefault(rank => rank.counts._id == id)?.rank * 100
                                / total.Count
                                + 1
                        )
                )
            );
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IDictionary<string, long?>> UnknownRanks()
        {
            var knownWords = await _ankiWords.GetAllKnownWords();
            var ranks = await _wordInfo.GetSortedWords(knownWords);
            return ranks.Select((word, index) => new WordRank(word, index * 100 / ranks.Count + 1))
                .ToDictionary(w => w.Id.ToString(), w => w.Rank);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<object> EpisodeTotal(
            [BindRequired] Analyzer analyzer,
            [BindRequired] int episodeId
        ) {
            var results = await _dialogCollectionManager.GetEpisodeWordCount(
                analyzer.ToAnalyzerMode(),
                episodeId
            );

            return results;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<WordOccurrences> Occurrences(
            int wordId,
            WordOccurrencesSortMode sortMode = WordOccurrencesSortMode.Default
        ) {
            switch (sortMode)
            {
                case WordOccurrencesSortMode.Default:
                {
                    var (occurrences, knownWords) = await (
                        _dialogCollectionManager.GetOccurrences(
                            Constants.DefaultAnalyzerMode,
                            wordId
                        ),
                        _wordInfo.GetWordRanks(await _ankiWords.GetAllKnownWords())
                    );

                    var knownWordsMap = knownWords.ToDictionary(
                        word => word.WordId,
                        word => word.GlobalRank
                    );

                    return new(
                        wordId,
                        occurrences.OrderByDescending(
                                occ => occ.wordIds.Sum(knownWordsMap.GetValueOrDefault)
                            )
                            .Select(oc => oc.dialogId)
                    );
                }
                case WordOccurrencesSortMode.Closest:
                    var specificities = await _dialogCollectionManager.GetOccurrencesSpecificities(
                        wordId
                    );
                    return new(
                        wordId,
                        specificities.OrderBy(specificity => specificity.Specificity)
                            .Select(specificity => specificity.DialogId)
                    );
                default:
                    throw new ArgumentOutOfRangeException(nameof(sortMode), sortMode, null);
            }
        }

        [HttpGet]
        [Route("known/{wordId}/[action]")]
        public async Task<IEnumerable<NoteInfo>> Notes(int wordId)
        {
            var words = await _ankiWords.GetWord(wordId);
            return words.Adapt<IEnumerable<NoteInfo>>();
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IReadOnlyDictionary<string, bool>> Known()
        {
            var words = await _ankiWords.GetAllKnownWords();
            return words.ToDictionary(w => w.ToString(), _ => true);
        }

        [HttpGet]
        [Route("[action]")]
        public Task<List<int>> WithReadingsKnown()
        {
            return _knownReadings.GetAllKnownReadings();
        }
    }
}
