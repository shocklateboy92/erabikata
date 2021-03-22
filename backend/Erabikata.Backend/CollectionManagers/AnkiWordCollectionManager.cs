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
            WordInfoCollectionManager wordInfoCollectionManager)
        {
            _ankiSyncClient = ankiSyncClient;
            _analyzerServiceClient = analyzerServiceClient;
            _logger = logger;
            _mongoCollection = database.GetCollection<AnkiWord>(
                nameof(AnkiWord)
            );
            _wordInfoCollectionManager = wordInfoCollectionManager;
        }

        public async Task<bool> IsWordInAnki(int wordId)
        {
            return await _mongoCollection.CountDocumentsAsync(
                word => word.WordId == wordId
            ) > 0;
        }

        public async Task OnActivityExecuting(Activity activity)
        {
            switch (activity)
            {
                case SyncAnki:
                    var (
                        _,
                        noteTexts,
                        matcher
                        ) = await (_mongoCollection.DeleteManyAsync(
                        FilterDefinition<AnkiWord>.Empty
                    ), GetAndAnalyzeNoteTexts(), _wordInfoCollectionManager.BuildWordMatcher());

                    var totalWords = new HashSet<int>();
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
                            totalWords.Add(word);
                        }
                    }

                    await _mongoCollection.InsertManyAsync(
                        totalWords.Select(wordId => new AnkiWord(wordId)),
                        new InsertManyOptions { IsOrdered = false }
                    );
                    break;
            }
        }

        private async Task<IEnumerable<(long id, Dialog.Word[])>> GetAndAnalyzeNoteTexts()
        {
            var notes = Unwrap(
                await _ankiSyncClient.FindNotes(
                    new AnkiAction(
                        "findNotes",
                        new { query = "\"note:Jap Sentences 2\"" }
                    )
                )
            );

            _logger.LogInformationString($"Got {notes.Length} notes for query");
            var noteInfos = Unwrap(
                await _ankiSyncClient.NotesInfo(
                    new AnkiAction("notesInfo", new { notes })
                )
            );

            _logger.LogInformationString($"Got {noteInfos.Length} notes info");
            var noteTexts = noteInfos.Select(
                note => (note.NoteId, ProcessText(
                    (note.Fields.GetValueOrDefault("Reading") ?? note.Fields[
                        "Text"
                    ]).Value
                ))
            );

            return await AnalyzeNoteTexts(noteTexts.ToArray());
        }

        private static readonly Regex ReadingsPattern = new Regex(
            @"\[[^]]*\]",
            RegexOptions.Compiled
        );
        private string ProcessText(string text) =>
            ReadingsPattern.Replace(text, string.Empty);

        private async Task<IEnumerable<(long id, Dialog.Word[])>> AnalyzeNoteTexts(
            IReadOnlyList<(long id, string text)> noteTexts)
        {
            var client = _analyzerServiceClient.AnalyzeBulk();
            await client.RequestStream.WriteAllAsync(
                noteTexts.Select(
                    note => new AnalyzeRequest
                    {
                        Mode = Constants.DefaultAnalyzerMode,
                        Text = note.text
                    }
                )
            );

            var results = await client.ResponseStream.ToListAsync();
            return results.Select(
                (result, index) => (noteTexts[index].id, result.Words.Select(
                        analyzed => analyzed.Adapt<Dialog.Word>()
                    )
                    .ToArray())
            );
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
