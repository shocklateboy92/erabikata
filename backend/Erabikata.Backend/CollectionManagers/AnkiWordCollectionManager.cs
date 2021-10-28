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
using MongoDB.Driver.Linq;
using TaskTupleAwaiter;

namespace Erabikata.Backend.CollectionManagers
{
    public class AnkiWordCollectionManager : ICollectionManager
    {
        private readonly IAnkiSyncClient _ankiSyncClient;

        private readonly WordInfoCollectionManager _wordInfoCollectionManager;
        private readonly AnalyzerService.AnalyzerServiceClient _analyzerServiceClient;
        private readonly ILogger<AnkiWordCollectionManager> _logger;
        private readonly IMongoCollection<AnkiNote> _mongoCollection;

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
            _mongoCollection = database.GetCollection<AnkiNote>(nameof(AnkiNote));
            _wordInfoCollectionManager = wordInfoCollectionManager;
        }

        public async Task<bool> IsWordInAnki(int wordId)
        {
            return await _mongoCollection.CountDocumentsAsync(word => word.WordIds.Contains(wordId))
                > 0;
        }

        public Task<List<int>> GetAllKnownWords()
        {
            return _mongoCollection.AsQueryable()
                .SelectMany(n => n.WordIds)
                .Distinct()
                .ToListAsync();
        }

        public Task<List<AnkiNote>> GetWord(int wordId)
        {
            return _mongoCollection.Find(note => note.WordIds.Contains(wordId)).ToListAsync();
        }

        public async Task OnActivityExecuting(Activity activity)
        {
            switch (activity)
            {
                case SyncAnki:
                    await _ankiSyncClient.Execute(new SyncAnkiAction());

                    var (noteTexts, takobotoWords, matcher) = await (
                        GetAndAnalyzeNoteTexts(),
                        GetTakobotoWords(),
                        _wordInfoCollectionManager.BuildWordMatcher()
                    );

                    var ankiWords = noteTexts.Select(
                            text =>
                                new AnkiNote(
                                    text.id,
                                    matcher.FillMatchesAndGetWords(text.Item2).ToArray(),
                                    text.primaryWord,
                                    text.primaryWordReading
                                )
                        )
                        .Concat(takobotoWords)
                        .ToArray();

                    // Only clear the collection once we have the replacements ready.
                    await _mongoCollection.DeleteManyAsync(FilterDefinition<AnkiNote>.Empty);

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

        private async Task<IEnumerable<AnkiNote>> GetTakobotoWords()
        {
            var notes = await FindAndGetNoteInfo("note:jp.takoboto");
            return notes.Select(
                note =>
                    new AnkiNote(
                        note.NoteId,
                        new[]
                        {
                            int.Parse(
                                TakobotoLinkPattern.Match(note.Fields["Link"].Value).Groups[1].Value
                            )
                        },
                        note.Fields["Japanese"].Value,
                        note.Fields["Reading"].Value
                    )
            );
        }

        private async Task<
            IEnumerable<(long id, Dialog.Word[], string primaryWord, string primaryWordReading)>
        > GetAndAnalyzeNoteTexts()
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

                    return (
                        note.NoteId,
                        ProcessText(text),
                        note.Fields["PrimaryWord"].Value,
                        note.Fields["PrimaryWordReading"].Value
                    );
                }
            );

            return await AnalyzeNoteTexts(noteTexts.ToArray());
        }

        private async Task<AnkiNoteResponse[]> FindAndGetNoteInfo(string query)
        {
            var notes = (
                await _ankiSyncClient.FindNotes(new AnkiAction("findNotes", new { query }))
            ).Unwrap();

            _logger.LogInformationString($"Got {notes.Length} notes for query");
            var noteInfos = (
                await _ankiSyncClient.NotesInfo(new AnkiAction("notesInfo", new { notes }))
            ).Unwrap();
            return noteInfos;
        }

        private static readonly Regex ReadingsPattern = new Regex(
            @"\[[^]]*\]",
            RegexOptions.Compiled
        );
        private static readonly Regex TagsPattern = new Regex(@"<[^>]*>", RegexOptions.Compiled);
        private string ProcessText(string text) =>
            TagsPattern.Replace(ReadingsPattern.Replace(text, string.Empty), " ");

        private async Task<
            IEnumerable<(long id, Dialog.Word[], string primaryWord, string primaryWordReading)>
        > AnalyzeNoteTexts(
            IReadOnlyList<(long id, string text, string primaryWord, string primaryWordReading)> noteTexts
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
                            result.Words.Select(analyzed => analyzed.Adapt<Dialog.Word>())
                                .ToArray(),
                            noteTexts[index].primaryWord,
                            noteTexts[index].primaryWordReading
                        );
                    }
                )
                .ToArray();
        }
    }
}
