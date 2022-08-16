using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Erabikata.Backend.Extensions;
using Erabikata.Backend.Models;
using Erabikata.Backend.Models.Actions;
using Erabikata.Backend.Models.Database;
using Grpc.Core;
using Grpc.Core.Utils;
using Mapster;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Erabikata.Backend.CollectionManagers;

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
    )
    {
        _ankiSyncClient = ankiSyncClient;
        _analyzerServiceClient = analyzerServiceClient;
        _logger = logger;
        _mongoCollection = database.GetCollection<AnkiNote>(nameof(AnkiNote));
        _wordInfoCollectionManager = wordInfoCollectionManager;
    }

    public Task<List<int>> GetAllKnownWords()
    {
        return _mongoCollection.AsQueryable().SelectMany(n => n.WordIds).Distinct().ToListAsync();
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

                var (takobotoWords, takobotoNoteTexts) = await GetTakobotoWords();
                var (noteTexts, matcher) = await (
                    GetAndAnalyzeNoteTexts(),
                    _wordInfoCollectionManager.BuildWordMatcher()
                );

                var ankiWords = noteTexts
                    .Concat(takobotoNoteTexts)
                    .Select(
                        text =>
                            new AnkiNote(
                                text.id,
                                matcher.FillMatchesAndGetWords(text.Item2).ToArray(),
                                text.primaryWord,
                                text.primaryWordReading,
                                text.Item2
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

    private async Task<(IEnumerable<AnkiNote>, IEnumerable<(long id, Dialog.Word[] words, string primaryWord, string primaryWordReading)>)> GetTakobotoWords()
    {
        var timeSpan = DateTime.Now - new DateTime(2021, 12, 4);
        var days = (int)timeSpan.TotalDays;
        var notes = await FindAndGetNoteInfo("note:jp.takoboto");
        var recentNotes = await FindNotes($"note:jp.takoboto -tag:unknown\\_words rated:{days}");
        var fullKnownWords = recentNotes.ToHashSet();
        return (
            notes
                .Where(n => !fullKnownWords.Contains(n.NoteId))
                .Select(
                    note =>
                        new AnkiNote(
                            note.NoteId,
                            new[]
                            {
                                int.Parse(
                                    TakobotoLinkPattern.Match(note.Fields["Link"].Value).Groups[
                                        1
                                    ].Value
                                )
                            },
                            note.Fields["Japanese"].Value,
                            note.Fields["Reading"].Value,
                            Array.Empty<Dialog.Word>()
                        )
                ),
            await AnalyzeNoteTexts(
                notes
                    .Where(n => fullKnownWords.Contains(n.NoteId))
                    .Select(
                        note =>
                            (
                                note.NoteId,
                                ProcessText(note.Fields["Sentence"].Value),
                                note.Fields["Japanese"].Value,
                                note.Fields["Reading"].Value
                            )
                    )
                    .ToArray()
            )
        );
    }

    private async Task<
        IEnumerable<(long id, Dialog.Word[] words, string primaryWord, string primaryWordReading)>
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
        var notes = await FindNotes(query);

        _logger.LogInformationString($"Got {notes.Length} notes for query");
        var noteInfos = (
            await _ankiSyncClient.NotesInfo(new AnkiAction("notesInfo", new { notes }))
        ).Unwrap();
        return noteInfos;
    }

    private async Task<long[]> FindNotes(string query)
    {
        return (
            await _ankiSyncClient.FindNotes(new AnkiAction("findNotes", new { query }))
        ).Unwrap();
    }

    private static readonly Regex ReadingsPattern = new Regex(@"\[[^]]*\]", RegexOptions.Compiled);
    private static readonly Regex TagsPattern = new Regex(@"<[^>]*>", RegexOptions.Compiled);

    private static string ProcessText(string text) =>
        TagsPattern
            .Replace(ReadingsPattern.Replace(text, string.Empty), string.Empty)
            .Replace(" ", string.Empty);

    private async Task<
        IEnumerable<(long id, Dialog.Word[], string primaryWord, string primaryWordReading)>
    > AnalyzeNoteTexts(
        IReadOnlyList<(long id, string text, string primaryWord, string primaryWordReading)> noteTexts
    )
    {
        var client = _analyzerServiceClient.AnalyzeBulk();
        foreach (var note in noteTexts)
        {
            await client.RequestStream.WriteAsync(
                new AnalyzeRequest { Mode = Constants.DefaultAnalyzerMode, Text = note.text }
            );
        }

        await client.RequestStream.CompleteAsync();

        var responses = await client.ResponseStream.ReadAllAsync().ToListAsync();
        return responses
            .Select(
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
                        result.Words.Select(analyzed => analyzed.Adapt<Dialog.Word>()).ToArray(),
                        noteTexts[index].primaryWord,
                        noteTexts[index].primaryWordReading
                    );
                }
            )
            .ToArray();
    }
}
