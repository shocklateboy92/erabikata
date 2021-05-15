using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Erabikata.Backend.Extensions;
using Erabikata.Backend.Models;
using Erabikata.Backend.Models.Actions;
using Erabikata.Backend.Models.Database;
using Grpc.Core.Utils;
using Mapster;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using TaskTupleAwaiter;

namespace Erabikata.Backend.CollectionManagers
{
    public class AnkiWordCollectionManager : ICollectionManager
    {
        private readonly IAnkiSyncClient _ankiSyncClient;

        private readonly WordInfoCollectionManager _wordInfoCollectionManager;
        private readonly AnalyzerService.AnalyzerServiceClient _analyzerServiceClient;
        private readonly ILogger<AnkiWordCollectionManager> _logger;
        private readonly IMongoCollection<AnkiWord> _mongoCollection;

        public AnkiWordCollectionManager(
            IAnkiSyncClient ankiSyncClient,
            IMongoDatabase database,
            AnalyzerService.AnalyzerServiceClient analyzerServiceClient,
            ILogger<AnkiWordCollectionManager> logger,
            WordInfoCollectionManager wordInfoCollectionManager
        ) {
            _ankiSyncClient = ankiSyncClient;
            _analyzerServiceClient = analyzerServiceClient;
            _logger = logger;
            _mongoCollection = database.GetCollection<AnkiWord>(nameof(AnkiWord));
            _wordInfoCollectionManager = wordInfoCollectionManager;
        }

        public async Task<bool> IsWordInAnki(int wordId)
        {
            return await _mongoCollection.CountDocumentsAsync(word => word.WordId == wordId) > 0;
        }

        public Task<List<int>> GetAllKnownWords()
        {
            return _mongoCollection.Find(FilterDefinition<AnkiWord>.Empty)
                .Project(word => word.WordId)
                .ToListAsync();
        }

        public async Task OnActivityExecuting(Activity activity)
        {
            switch (activity)
            {
                case SyncAnki:
                    var (noteTexts, takobotoWords, matcher) = await (
                        GetAndAnalyzeNoteTexts(),
                        GetTakobotoWords(),
                        _wordInfoCollectionManager.BuildWordMatcher()
                    );

                    var totalWords = new List<(int wordId, long noteId)>();
                    foreach (var (id, words) in noteTexts)
                    {
                        if (!words.Any())
                        {
                            _logger.LogWarning("nid:{nid} had no words", id);
                            continue;
                        }

                        var newWords = matcher.FillMatchesAndGetWords(words);
                        foreach (var word in newWords)
                        {
                            totalWords.Add((word, id));
                        }
                    }

                    totalWords.AddRange(takobotoWords);

                    var ankiWords = totalWords.GroupBy(
                            word => word.wordId,
                            (key, group) => new AnkiWord(key, group.Select(g => g.noteId))
                        )
                        .ToArray();

                    // Only clear the collection once we have the replacements ready.
                    await _mongoCollection.DeleteManyAsync(FilterDefinition<AnkiWord>.Empty);

                    await _mongoCollection.InsertManyAsync(
                        ankiWords,
                        new InsertManyOptions { IsOrdered = false }
                    );
                    break;
            }
        }

        private static readonly Regex TakobotoLinkPattern = new Regex(
            @"i\.word=(\d+);",
            RegexOptions.Compiled
        );

        private async Task<IEnumerable<(int wordId, long noteId)>> GetTakobotoWords()
        {
            var notes = await FindAndGetNoteInfo("note:jp.takoboto");
            return notes.Select(
                note =>
                    (
                        int.Parse(
                            TakobotoLinkPattern.Match(note.Fields["Link"].Value).Groups[1].Value
                        ),
                        note.NoteId
                    )
            );
        }

        private async Task<IEnumerable<(long id, Dialog.Word[])>> GetAndAnalyzeNoteTexts()
        {
            var noteInfos = await FindAndGetNoteInfo("\"note:Jap Sentences 2\"");
            _logger.LogInformationString($"Got {noteInfos.Length} notes info");
            var noteTexts = noteInfos.Select(
                note =>
                {
                    var text = note.Fields["Reading"].Value;
                    if (string.IsNullOrWhiteSpace(text))
                    {
                        text = note.Fields["Text"].Value;
                    }

                    return (note.NoteId, ProcessText(text));
                }
            );

            return await AnalyzeNoteTexts(noteTexts.ToArray());
        }

        private async Task<AnkiNote[]> FindAndGetNoteInfo(string query)
        {
            var notes = Unwrap(
                await _ankiSyncClient.FindNotes(new AnkiAction("findNotes", new { query }))
            );

            _logger.LogInformationString($"Got {notes.Length} notes for query");
            var noteInfos = Unwrap(
                await _ankiSyncClient.NotesInfo(new AnkiAction("notesInfo", new { notes }))
            );
            return noteInfos;
        }

        private static readonly Regex ReadingsPattern = new Regex(
            @"\[[^]]*\]",
            RegexOptions.Compiled
        );
        private static readonly Regex TagsPattern = new Regex(@"<[^>]*>", RegexOptions.Compiled);
        private string ProcessText(string text) =>
            TagsPattern.Replace(ReadingsPattern.Replace(text, string.Empty), " ");

        private async Task<IEnumerable<(long id, Dialog.Word[])>> AnalyzeNoteTexts(
            IReadOnlyList<(long id, string text)> noteTexts
        ) {
            var client = _analyzerServiceClient.AnalyzeBulk();
            await client.RequestStream.WriteAllAsync(
                noteTexts.Select(
                    note =>
                        new AnalyzeRequest
                        {
                            Mode = Constants.DefaultAnalyzerMode,
                            Text = note.text
                        }
                )
            );

            var responses = await client.ResponseStream.ToListAsync();
            return responses.Select(
                    (result, index) =>
                    {
                        _logger.LogDebug(
                            "Completed analysis for note {nid}:\n\tInput: {text}\n\tAnalyzed: {words})",
                            noteTexts[index].id,
                            noteTexts[index].text,
                            result.Words.Select(word => word.Original)
                        );
                        return (
                            noteTexts[index].id,
                            result.Words.Select(analyzed => analyzed.Adapt<Dialog.Word>()).ToArray()
                        );
                    }
                )
                .ToArray();
        }

        private static T Unwrap<T>(AnkiResponse<T> ankiResponse)
        {
            if (!string.IsNullOrEmpty(ankiResponse.Error))
            {
                throw new AnkiConnectException(ankiResponse.Error);
            }

            return ankiResponse.Result;
        }
    }
}
