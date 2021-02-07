using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
            }
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
                            (info, index) => IngestEpisode(
                                files.FirstOrDefault(
                                    path => path.EndsWith($"input/{index + 1:00}.ass") ||
                                            path.EndsWith($"input/{index + 1:00}.srt")
                                ),
                                (index + 1, info.Key),
                                includeStyles,
                                showInfo.Title
                            )
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

            foreach (var (_, collection) in _mongoCollections)
            {
                using var analyzer = _analyzerServiceClient.AnalyzeDialogBulk();
                await analyzer.RequestStream.WriteAllAsync(
                    toInclude.Select(
                        responseDialog =>
                        {
                            var request = responseDialog.Adapt<AnalyzeDialogRequest>();
                            request.Lines.AddRange(responseDialog.Lines);
                            return request;
                        }
                    )
                );
                var analyzed = await analyzer.ResponseStream.ToListAsync();
                await collection.InsertManyAsync(
                    analyzed.Select(
                        response =>
                            new Dialog(
                                ObjectId.Empty,
                                episodeId,
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
                        baseForm: word.BaseForm,
                        dictionaryForm: word.DictionaryForm,
                        originalForm: word.Original,
                        reading: word.Reading,
                        isInParenthesis: bracketCount > 0
                    ) {PartOfSpeech = word.PartOfSpeech}
                );
            }

            return new Dialog.Line(results);
        }

        private const int TimeDelta = 10;

        public Task<List<Dialog>> GetNearestDialog(
            int episodeId,
            double time,
            int count,
            AnalyzerMode analyzerMode) =>
            _mongoCollections[analyzerMode]
                .Find(
                    dialog => dialog.EpisodeId == episodeId && dialog.Time > time - TimeDelta &&
                              dialog.Time < time + TimeDelta
                )
                .SortBy(dialog => dialog.Time)
                .ToListAsync();

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