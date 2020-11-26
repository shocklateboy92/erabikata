using System;
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
    public class WordController : ControllerBase
    {
        private readonly SubtitleDatabaseManager _database;
        private readonly EpisodeInfoManager _episodeInfoManager;
        private readonly WordCountsManager _wordCounts;
        private readonly KnownWordsProvider _knownWordsProvider;

        public WordController(
            SubtitleDatabaseManager database,
            WordCountsManager wordCounts,
            EpisodeInfoManager episodeInfoManager,
            KnownWordsProvider knownWordsProvider)
        {
            _database = database;
            _wordCounts = wordCounts;
            _episodeInfoManager = episodeInfoManager;
            _knownWordsProvider = knownWordsProvider;
        }

        [HttpGet]
        [Route("{text}")]
        public async Task<ActionResult<WordInfo>> Index(
            [FromRoute] string text,
            [FromQuery] HashSet<PartOfSpeech> onlyPartsOfSpeech,
            string? includeEpisode,
            double? includeTime,
            [FromQuery] PagingInfo pagingInfo)
        {
            var rank = _wordCounts.WordRanksMap.GetValueOrDefault(text, -1);
            if (rank < 0)
            {
                return NotFound(
                    new WordInfo
                    {
                        Text = text,
                        TotalOccurrences = 0,
                        Occurrences = new WordInfo.Occurence[] { }
                    }
                );
            }

            var occurrences = _database.AllEpisodes.SelectMany(
                episode =>
                {
                    var episodeInfo = _episodeInfoManager.GetEpisodeInfo(episode);
                    return episode.Dialog.SelectMany(
                        sentence => sentence.Analyzed
                            .Where(
                                word => word.Base == text && (onlyPartsOfSpeech.Count == 0 ||
                                                              word.PartOfSpeech.Any(
                                                                  onlyPartsOfSpeech.Contains
                                                              ))
                            )
                            // Easiest ways to avoid duplicates in sentence
                            .Take(1)
                            .Select(
                                word => new WordInfo.Occurence
                                {
                                    EpisodeName = episodeInfo.Title,
                                    EpisodeId = episode.Title,
                                    Time = sentence.StartTime,
                                    Text = new DialogInfo(sentence),
                                    VlcCommand =
                                        episodeInfo.VideoFile != null
                                            ? $"vlc '{episodeInfo.VideoFile}' --start-time={sentence.StartTime - 10}"
                                            : null,
                                    SubsLink = Url.Action(
                                        "Index",
                                        "Subs",
                                        new
                                        {
                                            time = sentence.StartTime,
                                            episode = episode.Title
                                        },
                                        HttpContext.Request.Scheme
                                    )
                                }
                            )
                    );
                }
            );

            if (includeEpisode != null && includeTime != null)
            {
                var ranksMap = _wordCounts.WordRanksMap;
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
                            word => knownWords.Contains(word.BaseForm)
                                ? ranksMap.GetValueOrDefault(word.BaseForm, 0)
                                : 0
                        );
                    }
                );
            }

            return new WordInfo
            {
                Text = text,
                Rank = rank,
                TotalOccurrences = _wordCounts.WordRanks[rank].count,
                Occurrences = occurrences.Skip(pagingInfo.Skip).Take(pagingInfo.Max),
                PagingInfo = pagingInfo
            };
        }

        [HttpGet]
        [Route("{word}/[action]")]
        public IEnumerable<PartOfSpeech> PartsOfSpeech([FromRoute] string word)
        {
            return _database.AllEpisodes.SelectMany(
                    ep => ep.Dialog.SelectMany(
                        line => line.Analyzed.Where(w => w.Base == word)
                            .SelectMany(w => w.PartOfSpeech)
                    )
                )
                .Distinct();
        }
    }
}