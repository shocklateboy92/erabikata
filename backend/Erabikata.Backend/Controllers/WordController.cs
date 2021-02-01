using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Erabikata.Backend.CollectionManagers;
using Erabikata.Backend.Managers;
using Erabikata.Models.Configuration;
using Erabikata.Models.Input;
using Erabikata.Models.Output;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TaskTupleAwaiter;

namespace Erabikata.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WordController : ControllerBase
    {
        private readonly SubtitleDatabaseManager _database;
        private readonly EpisodeInfoManager _episodeInfoManager;
        private readonly WordCountsManager _wordCounts;
        private readonly WordStateManager _knownWordsProvider;
        private readonly SubtitleProcessingSettings _processingSettings;
        private readonly DialogCollectionManager _dialogCollectionManager;

        public WordController(
            SubtitleDatabaseManager database,
            WordCountsManager wordCounts,
            EpisodeInfoManager episodeInfoManager,
            WordStateManager knownWordsProvider,
            IOptions<SubtitleProcessingSettings> processingSettings,
            DialogCollectionManager dialogCollectionManager)
        {
            _database = database;
            _wordCounts = wordCounts;
            _episodeInfoManager = episodeInfoManager;
            _knownWordsProvider = knownWordsProvider;
            _dialogCollectionManager = dialogCollectionManager;
            _processingSettings = processingSettings.Value;
        }

        [HttpGet]
        [Route("{text}")]
        public async Task<WordInfo> Index(
            [FromRoute] string text,
            [FromQuery] HashSet<string> onlyPartsOfSpeech,
            string? includeEpisode,
            double? includeTime,
            [FromQuery] PagingInfo pagingInfo,
            Analyzer analyzer = Analyzer.Kuromoji)
        {
            var rank = _wordCounts.WordRanksMap[analyzer].GetValueOrDefault(text, -1);
            if (rank < 0)
            {
                return new WordInfo
                {
                    Text = text,
                    TotalOccurrences = 0,
                    Occurrences = new WordInfo.Occurence[] { }
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
        public IEnumerable<string> PartsOfSpeech([FromRoute] string word, Analyzer analyzer)
        {
            var words = _database.AllEpisodesV2.Values.SelectMany(
                v2 => v2.AnalyzedSentences[analyzer]
                    .SelectMany(
                        sentenceV2 => sentenceV2.Analyzed.SelectMany(
                            line => line.Where(analyzedWord => analyzedWord.Base == word)
                                .SelectMany(analyzed => analyzed.PartOfSpeech)
                        )
                    )
            );
            if (analyzer == Analyzer.Kuromoji)
            {
                words = _database.AllEpisodes.SelectMany(
                    ep => ep.Dialog.SelectMany(
                        line => line.Analyzed.Where(w => w.Base == word)
                            .SelectMany(w => w.PartOfSpeech)
                    )
                );
            }

            return words.Distinct();
        }
    }
}