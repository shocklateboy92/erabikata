using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Erabikata.Backend.DataProviders;
using Erabikata.Backend.Managers;
using Erabikata.Models.Configuration;
using Erabikata.Models.Input;
using Erabikata.Models.Output;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Erabikata.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WordController : ControllerBase
    {
        private readonly SubtitleDatabaseManager _database;
        private readonly EpisodeInfoManager _episodeInfoManager;
        private readonly WordCountsManager _wordCounts;
        private readonly KnownWordsProvider _knownWordsProvider;
        private readonly SubtitleProcessingSettings _processingSettings;

        public WordController(
            SubtitleDatabaseManager database,
            WordCountsManager wordCounts,
            EpisodeInfoManager episodeInfoManager,
            KnownWordsProvider knownWordsProvider,
            IOptions<SubtitleProcessingSettings> processingSettings)
        {
            _database = database;
            _wordCounts = wordCounts;
            _episodeInfoManager = episodeInfoManager;
            _knownWordsProvider = knownWordsProvider;
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

            bool IsTargetWord(Analyzed word) =>
                word.Base == text && (onlyPartsOfSpeech.Count == 0 ||
                                      word.PartOfSpeech.Any(onlyPartsOfSpeech.Contains));


            var occurrences = _database.AllEpisodesV2.Values.SelectMany(
                episode => episode.FilteredInputSentences
                    .Where(
                        sentence => episode.AnalyzedSentences[analyzer][sentence.Index]
                            .Analyzed.Any(line => line.Any(IsTargetWord))
                    )
                    .Select(
                        sentence => new WordInfo.Occurence
                        {
                            EpisodeId = episode.Id.ToString(),
                            EpisodeName = $"{episode.Parent.Title} Episode {episode.Number}",
                            Text = new DialogInfo(
                                sentence,
                                episode.AnalyzedSentences[analyzer][sentence.Index],
                                _processingSettings.IgnoredPartsOfSpeech
                            ),
                            Time = sentence.Time,
                            VlcCommand = CreateVlcCommand(episode.FilePath, sentence.Time)
                        }
                    )
            );

            if (analyzer == Analyzer.Kuromoji)
            {
                occurrences = occurrences.Concat(
                    _database.AllEpisodes.SelectMany(
                        episode =>
                        {
                            var episodeInfo = _episodeInfoManager.GetEpisodeInfo(episode);
                            return episode.Dialog.SelectMany(
                                sentence =>
                                {
                                    return sentence.Analyzed.Where(IsTargetWord)
                                        // Easiest ways to avoid duplicates in sentence
                                        .Take(1)
                                        .Select(
                                            word => new WordInfo.Occurence
                                            {
                                                EpisodeName = episodeInfo.Title,
                                                EpisodeId = episode.Title,
                                                Time = sentence.StartTime,
                                                Text = new DialogInfo(sentence),
                                                VlcCommand = episodeInfo.VideoFile != null
                                                    ? CreateVlcCommand(
                                                        episodeInfo.VideoFile,
                                                        sentence.StartTime
                                                    )
                                                    : null,
                                            }
                                        );
                                }
                            );
                        }
                    )
                );
            }

            if (includeEpisode != null && includeTime != null)
            {
                var ranksMap = _wordCounts.WordRanksMap[analyzer];
                var knownWords = await _knownWordsProvider.GetKnownWords();

                occurrences = occurrences.OrderByDescending(
                    occurence =>
                    {
                        if (occurence.Time.Equals(includeTime) &&
                            occurence.EpisodeId == includeEpisode)
                        {
                            return int.MaxValue;
                        }

                        return occurence.Text.Words.Sum(
                            line => line.Sum(
                                word => knownWords.Contains(word.BaseForm)
                                    ? ranksMap.GetValueOrDefault(word.BaseForm, 0)
                                    : 0
                            )
                        );
                    }
                );
            }

            var occurrencesList = occurrences.ToList();
            return new WordInfo
            {
                Text = text,
                Rank = rank,
                TotalOccurrences = occurrencesList.Count,
                Occurrences = occurrencesList.Skip(pagingInfo.Skip).Take(pagingInfo.Max),
                PagingInfo = pagingInfo
            };
        }

        private static string CreateVlcCommand(string path, double time) =>
            $"vlc '{path}' --start-time={time - 10}";

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