using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Erabikata.Backend.DataProviders;
using Erabikata.Backend.Models.Actions;
using Erabikata.Backend.Models.Database;
using Erabikata.Models.Input.V2;
using Grpc.Core;
using Grpc.Core.Utils;
using Mapster;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Activity = Erabikata.Backend.Models.Actions.Activity;

namespace Erabikata.Backend.CollectionManagers
{
    public class DialogCollectionManager : ICollectionManager
    {
        private readonly IReadOnlyDictionary<AnalyzerMode, IMongoCollection<Dialog>>
            _mongoCollections;

        private readonly ILogger<DialogCollectionManager> _logger;
        private readonly AssParserService.AssParserServiceClient _assParserServiceClient;
        private readonly AnalyzerService.AnalyzerServiceClient _analyzerServiceClient;

        public DialogCollectionManager(
            IMongoDatabase database,
            ILogger<DialogCollectionManager> logger,
            AnalyzerService.AnalyzerServiceClient analyzerServiceClient,
            AssParserService.AssParserServiceClient assParserServiceClient)
        {
            _logger = logger;
            _analyzerServiceClient = analyzerServiceClient;
            _assParserServiceClient = assParserServiceClient;
            _mongoCollections =
                new[] {AnalyzerMode.SudachiA, AnalyzerMode.SudachiB, AnalyzerMode.SudachiC}
                    .ToDictionary(
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
                case DictionaryIngestion ({ } words):
                    // Parallel.ForEachAsync(words,
                    //     async (info, token) =>
                    //     {
                    //         
                    //     })
                    break;
            }
        }

        public async Task ProcessWords(IReadOnlyList<WordInfo> words)
        {
            var cursor = await _mongoCollections[AnalyzerMode.SudachiC]
                .FindAsync(
                    dialog => dialog.EpisodeId == 2056,
                    new FindOptions<Dialog> {BatchSize = 1000}
                );
            while (await cursor.MoveNextAsync())
            {
                var processed = cursor.Current.AsParallel()
                    .Select(
                        dialog =>
                        {
                            foreach (var wordInfo in words)
                            foreach (var line in dialog.Lines)
                            {
                                var normalized = wordInfo.Normalized[0];
                                var startingIndexes = StartingIndex(line.Words, normalized);

                                foreach (var startingIndex in startingIndexes)
                                    for (var index = 0; index < normalized.Count; index++)
                                    {
                                        line.Words[startingIndex + index].InfoIds.Add(wordInfo.Id);
                                    }
                            }

                            return dialog;
                        }
                    );

                var replaceOneModels = processed.Select(
                        dialog => new ReplaceOneModel<Dialog>(
                            Builders<Dialog>.Filter.Eq(d => d.Id, dialog.Id),
                            dialog
                        )
                    )
                    .ToArray();
                await _mongoCollections[AnalyzerMode.SudachiC].BulkWriteAsync(replaceOneModels);
            }
        }

        private static IEnumerable<int> StartingIndex(
            IReadOnlyList<Dialog.Word> input,
            IReadOnlyList<string> query)
        {
            var maxMatches = input.Count - query.Count + 1;
            if (maxMatches < 0)
            {
                return Enumerable.Empty<int>();
            }

            var startingIndexes = Enumerable.Range(0, maxMatches);
            for (var i = 0; i < query.Count; i++)
            {
                startingIndexes = startingIndexes
                    .Where(start => input[start + i].BaseForm == query[i])
                    .ToArray();
            }

            return startingIndexes;
        }

        private async Task IngestDialog(
            IEnumerable<IngestShows.ShowToIngest> ingestShowsShowsToIngest)
        {
            foreach (var (files, showInfo) in ingestShowsShowsToIngest)
            {
                var includeStylesFile =
                    files.FirstOrDefault(path => path.EndsWith("input/include_styles.txt"));
                var includeStyles = includeStylesFile == null
                    ? new HashSet<string>()
                    : (await File.ReadAllLinesAsync(includeStylesFile)).ToHashSet();

                await Task.WhenAll(
                    showInfo.Episodes[0]
                        .Select(
                            (info, index) =>
                            {
                                var epNum = index + 1;
                                return IngestEpisode(
                                    files.FirstOrDefault(
                                        path => SeedDataProvider.IsPathForEpisode(
                                            path,
                                            "input",
                                            epNum
                                        )
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
            string showTitle)
        {
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
                    responseDialog => !responseDialog.IsComment &&
                                      (includeStyles.Contains(responseDialog.Style) ||
                                       file.EndsWith(".srt"))
                )
                .ToList();

            foreach (var (mode, collection) in _mongoCollections)
            {
                using var analyzer = _analyzerServiceClient.AnalyzeDialogBulk();
                await analyzer.RequestStream.WriteAllAsync(
                    toInclude.Select(
                        responseDialog => new AnalyzeDialogRequest
                        {
                            Mode = mode,
                            Style = responseDialog.Style,
                            Time = responseDialog.Time,
                            Lines = {responseDialog.Lines}
                        }
                    )
                );
                var analyzed = await analyzer.ResponseStream.ToListAsync();
                await collection.InsertManyAsync(
                    analyzed.Select(
                        (response, index) => new Dialog(
                            ObjectId.Empty,
                            episodeId,
                            index: index,
                            time: response.Time,
                            episodeTitle: $"{showTitle} Episode {info.index}"
                        ) {Lines = response.Lines.Select(ProcessLine)}
                    ),
                    new InsertManyOptions {IsOrdered = false}
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
                {
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
                }

                results.Add(
                    new Dialog.Word(
                        BaseForm: word.BaseForm,
                        DictionaryForm: word.DictionaryForm,
                        OriginalForm: word.Original,
                        Reading: word.Reading,
                        IsInParenthesis: bracketCount > 0
                    ) {PartOfSpeech = word.PartOfSpeech}
                );
            }

            return new Dialog.Line(results);
        }

        private const int TimeDelta = 10;

        public async Task<IReadOnlyList<Dialog>> GetNearestDialog(
            int episodeId,
            double time,
            int count,
            AnalyzerMode analyzerMode)
        {
            var closest = await _mongoCollections[analyzerMode]
                .Aggregate()
                .Match(dialog => dialog.EpisodeId == episodeId)
                .Project(dialog => new {dialog.Index, Delta = Math.Abs(dialog.Time - time)})
                .SortBy(d => d.Delta)
                .FirstOrDefaultAsync();

            return await _mongoCollections[analyzerMode]
                .Find(
                    dialog => dialog.EpisodeId == episodeId &&
                              dialog.Index > closest.Index - count &&
                              dialog.Index < closest.Index + count
                )
                .ToListAsync();
        }

        public Task<List<Dialog>> GetMatches(
            string baseForm,
            AnalyzerMode analyzerMode,
            int skip = 0,
            int take = int.MaxValue) =>
            Find(baseForm, analyzerMode).Skip(skip).Limit(take).ToListAsync();

        public Task<List<Dialog>>
            GetFuzzyMatches(string baseOrDictionaryForm, AnalyzerMode analyzerMode) =>
            _mongoCollections[analyzerMode]
                .Find(
                    dialog => dialog.Lines.Any(
                        line => line.Words.Any(
                            word => word.BaseForm.Contains(baseOrDictionaryForm) ||
                                    word.DictionaryForm.Contains(baseOrDictionaryForm)
                        )
                    )
                )
                .ToListAsync();

        public Task<long> CountMatches(string baseForm, AnalyzerMode mode) =>
            Find(baseForm, mode).CountDocumentsAsync();

        public Task<List<AggregateSortByCountResult<string>>> GetSortedWordCounts(
            AnalyzerMode mode,
            IEnumerable<string> ignoredPartsOfSpeech,
            int max = int.MaxValue,
            int skip = 0) =>
            _mongoCollections[mode]
                .Aggregate()
                .Unwind(dialog => dialog.Lines)
                .Unwind<IntermediateDialog>($"{nameof(Dialog.Lines)}.{nameof(Dialog.Line.Words)}")
                .Match(
                    dialog => dialog.Lines.Words.IsInParenthesis == false &&
                              !dialog.Lines.Words.PartOfSpeech.Any(
                                  // ReSharper disable once ConvertClosureToMethodGroup
                                  // LINQ to Mongo Query generator can't handle method groups
                                  pos => ignoredPartsOfSpeech.Contains(pos)
                              )
                )
                .SortByCount<string>(
                    $"${nameof(Dialog.Lines)}.{nameof(Dialog.Line.Words)}.{nameof(Dialog.Word.BaseForm)}"
                )
                .Skip(skip)
                .Limit(max)
                .ToListAsync();

        private record IntermediateLine(Dialog.Word Words);

        private record IntermediateDialog(IntermediateLine Lines);

        public record WordRank(
            [property: BsonId] string BaseForm,
            [property: BsonElement("count")] long Count);

        private IFindFluent<Dialog, Dialog> Find(string baseForm, AnalyzerMode analyzerMode) =>
            _mongoCollections[analyzerMode]
                .Find(
                    dialog => dialog.Lines.Any(
                        line => line.Words.Any(word => word.BaseForm == baseForm)
                    )
                );
    }
}