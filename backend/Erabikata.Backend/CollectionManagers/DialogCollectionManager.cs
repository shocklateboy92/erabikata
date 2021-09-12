﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Erabikata.Backend.DataProviders;
using Erabikata.Backend.Models.Actions;
using Erabikata.Backend.Models.Database;
using Erabikata.Backend.Models.Output;
using Erabikata.Backend.Processing;
using Erabikata.Backend.Stolen;
using Grpc.Core.Utils;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Erabikata.Backend.CollectionManagers
{
    public class DialogCollectionManager : ICollectionManager
    {
        private const int TimeDelta = 10;
        private readonly AnalyzerService.AnalyzerServiceClient _analyzerServiceClient;
        private readonly AssParserService.AssParserServiceClient _assParserServiceClient;

        private readonly ILogger<DialogCollectionManager> _logger;

        private readonly IReadOnlyDictionary<
            AnalyzerMode,
            IMongoCollection<Dialog>
        > _mongoCollections;

        public DialogCollectionManager(
            IMongoDatabase database,
            ILogger<DialogCollectionManager> logger,
            AnalyzerService.AnalyzerServiceClient analyzerServiceClient,
            AssParserService.AssParserServiceClient assParserServiceClient
        ) {
            _logger = logger;
            _analyzerServiceClient = analyzerServiceClient;
            _assParserServiceClient = assParserServiceClient;
            _mongoCollections = new[]
            {
                AnalyzerMode.SudachiA,
                AnalyzerMode.SudachiB,
                AnalyzerMode.SudachiC
            }.ToDictionary(
                mode => mode,
                mode => database.GetCollection<Dialog>(nameof(Dialog) + mode)
            );
        }

        public async Task OnActivityExecuting(Activity activity)
        {
            switch (activity)
            {
                case IngestShows ingestShows:
                    await Task.WhenAll(
                        _mongoCollections.Values.Select(
                            collection => collection.DeleteManyAsync(FilterDefinition<Dialog>.Empty)
                        )
                    );
                    await IngestDialog(ingestShows.ShowsToIngest);
                    break;
            }
        }

        public async Task ProcessWords2(WordMatcher matcher)
        {
            var cursor = await _mongoCollections[Constants.DefaultAnalyzerMode].FindAsync(
                FilterDefinition<Dialog>.Empty,
                new FindOptions<Dialog> { BatchSize = 10000 }
            );
            while (await cursor.MoveNextAsync())
            {
                var processed = cursor.Current.AsParallel()
                    .Select(
                        dialog =>
                        {
                            foreach (var line in dialog.Lines)
                            {
                                var wordsInLine = matcher.FillMatchesAndGetWords(line.Words);
                                foreach (var wordId in wordsInLine)
                                {
                                    dialog.WordsToRank.Add(wordId);
                                }
                            }

                            return dialog;
                        }
                    );

                var replaceOneModels = processed.Select(
                        dialog =>
                            new ReplaceOneModel<Dialog>(
                                Builders<Dialog>.Filter.Eq(d => d.Id, dialog.Id),
                                dialog
                            )
                    )
                    .ToArray();
                await _mongoCollections[Constants.DefaultAnalyzerMode].BulkWriteAsync(
                    replaceOneModels,
                    new BulkWriteOptions { IsOrdered = false }
                );
            }
        }

        private async Task IngestDialog(
            IEnumerable<IngestShows.ShowToIngest> ingestShowsShowsToIngest
        ) {
            foreach (var (files, showInfo) in ingestShowsShowsToIngest)
            {
                var includeStylesFile = files.FirstOrDefault(
                    path => path.EndsWith("input/include_styles.txt")
                );
                var includeStyles =
                    includeStylesFile == null
                        ? new HashSet<string>()
                        : (await File.ReadAllLinesAsync(includeStylesFile)).ToHashSet();

                await Task.WhenAll(
                    showInfo.Episodes[0].Select(
                        (info, index) =>
                        {
                            var epNum = index + 1;
                            return IngestEpisode(
                                files.FirstOrDefault(
                                    path => SeedDataProvider.IsPathForEpisode(path, "input", epNum)
                                ),
                                (epNum, info.Key),
                                includeStyles,
                                showInfo.Title
                            );
                        }
                    )
                );
            }
        }

        private async Task IngestEpisode(
            string? file,
            (int index, string key) info,
            IReadOnlySet<string> includeStyles,
            string showTitle
        ) {
            var episodeId = int.Parse(info.key.Split('/').Last());
            if (file == null)
            {
                _logger.LogError("Unable to find input file for episode '{EpisodeId}'", info);
                return;
            }

            using var client = _assParserServiceClient.ParseAss();
            await EngSubCollectionManager.WriteFileToParserClient(client, file);

            var dialog = await client.ResponseStream.ToListAsync();
            var toInclude = dialog.Where(
                    responseDialog =>
                        !responseDialog.IsComment
                        && (includeStyles.Contains(responseDialog.Style) || file.EndsWith(".srt"))
                )
                .ToList();

            foreach (var (mode, collection) in _mongoCollections)
            {
                using var analyzer = _analyzerServiceClient.AnalyzeDialogBulk();
                await analyzer.RequestStream.WriteAllAsync(
                    toInclude.Select(
                        responseDialog =>
                            new AnalyzeDialogRequest
                            {
                                Mode = mode,
                                Style = responseDialog.Style,
                                Time = responseDialog.Time,
                                Lines = { responseDialog.Lines }
                            }
                    )
                );
                var analyzed = await analyzer.ResponseStream.ToListAsync();
                await collection.InsertManyAsync(
                    analyzed.Select(
                        (response, index) =>
                            new Dialog(
                                ObjectId.Empty,
                                episodeId,
                                index,
                                response.Time,
                                $"{showTitle} Episode {info.index}"
                            ) {
                                Lines = response.Lines.Select(ProcessLine)
                            }
                    ),
                    new InsertManyOptions { IsOrdered = false }
                );
            }
        }

        private static Dialog.Line ProcessLine(AnalyzeDialogResponse.Types.Line line)
        {
            var results = new List<Dialog.Word>(line.Words.Count);
            foreach (var word in line.Words)
            {
                var bracketCount = 0;
                foreach (var c in word.Original)
                    switch (c)
                    {
                        case '(':
                        case '（':
                            bracketCount++;
                            break;
                        case ')':
                        case '）':
                            bracketCount--;
                            break;
                    }

                results.Add(
                    new Dialog.Word(
                        word.BaseForm,
                        word.DictionaryForm,
                        // replace the normalized spaces with the funky Japanese spaces
                        word.Original.Replace(' ', '　'),
                        word.Reading,
                        bracketCount > 0
                    ) {
                        PartOfSpeech = word.PartOfSpeech
                    }
                );
            }

            return new Dialog.Line(results);
        }

        public async Task<IReadOnlyList<UnwoundRank>> GetWordRanks(
            AnalyzerMode mode,
            int episodeId,
            IEnumerable<int> wordIds
        ) {
            var cursor = await _mongoCollections[mode].AggregateAsync(
                PipelineDefinition<Dialog, UnwoundRank>.Create(
                    new BsonDocument("$match", new BsonDocument("EpisodeId", episodeId)),
                    new BsonDocument("$unwind", "$WordsToRank"),
                    new BsonDocument("$sortByCount", "$WordsToRank"),
                    new BsonDocument(
                        "$group",
                        new BsonDocument
                        {
                            { "_id", BsonNull.Value },
                            { "counts", new BsonDocument("$push", "$$ROOT") }
                        }
                    ),
                    new BsonDocument(
                        "$unwind",
                        new BsonDocument { { "path", "$counts" }, { "includeArrayIndex", "rank" } }
                    ),
                    new BsonDocument(
                        "$match",
                        new BsonDocument(
                            "counts._id",
                            new BsonDocument("$in", new BsonArray(wordIds))
                        )
                    )
                )
            );
            return await cursor.ToListAsync();
        }

        public Task<AggregateCountResult> GetEpisodeWordCount(
            AnalyzerMode analyzerMode,
            int episodeId
        ) {
            return _mongoCollections[analyzerMode].Aggregate()
                .Match(dialog => dialog.EpisodeId == episodeId)
                .Unwind<Dialog, UnwoundDialog>(dialog => dialog.WordsToRank)
                .Group(
                    doc => string.Empty,
                    grouping =>
                        new { uniqueWordIds = grouping.Select(i => i.WordsToRank).Distinct() }
                )
                .Unwind(group => group.uniqueWordIds)
                .Count()
                .FirstAsync();
        }

        public Task<List<DialogWords>> GetOccurrences(AnalyzerMode mode, int wordId)
        {
            return _mongoCollections[mode].Find(
                    dialog =>
                        dialog.Lines.Any(
                            line => line.Words.Any(word => word.InfoIds.Contains(wordId))
                        )
                )
                .Project(dialog => new DialogWords(dialog.Id.ToString(), dialog.WordsToRank))
                .ToListAsync();
        }

        public Task<List<Dialog>> GetByIds(AnalyzerMode mode, IEnumerable<string> dialogId)
        {
            return _mongoCollections[mode].Find(
                    dialog => dialogId.Select(ObjectId.Parse).Contains(dialog.Id)
                )
                .ToListAsync();
        }

        public Task<string> GetEpisodeTitle(AnalyzerMode mode, int episodeId)
        {
            return _mongoCollections[mode].Find(dialog => dialog.EpisodeId == episodeId)
                .Project(dialog => dialog.EpisodeTitle)
                .FirstOrDefaultAsync();
        }

        public Task<List<Episode.Entry>> GetEpisodeDialog(AnalyzerMode mode, int episodeId)
        {
            return _mongoCollections[mode].Find(dialog => dialog.EpisodeId == episodeId)
                .Project(dialog => new Episode.Entry(dialog.Time, dialog.Id.ToString()))
                .SortBy(entry => entry.Time)
                .ToListAsync();
        }

        public record UnwoundRank(object? _id, int rank, UnwoundWordCount counts);

        public record UnwoundWordCount(int _id, int count);

        private record IntermediateLine(Dialog.Word Words);

        private record IntermediateDialog(IntermediateLine Lines);

        public record DialogWords(string dialogId, IEnumerable<int> wordIds);

        public record WordRank(
            [property: BsonId] string BaseForm,
            [property: BsonElement("count")] long Count
        );

        private record UnwoundDialog(int WordsToRank);
    }
}
