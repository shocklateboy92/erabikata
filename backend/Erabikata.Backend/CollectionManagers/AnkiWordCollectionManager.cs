using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Erabikata.Backend.Extensions;
using Erabikata.Backend.Models;
using Erabikata.Backend.Models.Actions;
using Erabikata.Backend.Models.Database;
using Grpc.Core.Utils;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;

namespace Erabikata.Backend.CollectionManagers
{
    public class AnkiWordCollectionManager : ICollectionManager
    {
        private readonly IAnkiSyncClient _ankiSyncClient;
        private readonly AnalyzerService.AnalyzerServiceClient _analyzerServiceClient;
        private readonly ILogger<AnkiWordCollectionManager> _logger;
        private readonly IMongoCollection<AnkiWord> _mongoCollection;

        public AnkiWordCollectionManager(
            IAnkiSyncClient ankiSyncClient,
            IMongoDatabase database,
            AnalyzerService.AnalyzerServiceClient analyzerServiceClient,
            ILogger<AnkiWordCollectionManager> logger)
        {
            _ankiSyncClient = ankiSyncClient;
            _analyzerServiceClient = analyzerServiceClient;
            _logger = logger;
            _mongoCollection = database.GetCollection<AnkiWord>(
                nameof(AnkiWord)
            );
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
                    await _mongoCollection.DeleteManyAsync(
                        FilterDefinition<AnkiWord>.Empty
                    );
                    var notes = Unwrap(
                        await _ankiSyncClient.FindNotes(
                            new AnkiAction(
                                "findNotes",
                                new { query = "\"note:Jap Sentences 2\"" }
                            )
                        )
                    );

                    _logger.LogInformationString($"Got {notes.Length} notes for query");
                    var noteInfo = Unwrap(
                        await _ankiSyncClient.NotesInfo(
                            new AnkiAction("notesInfo", new { notes })
                        )
                    );

                    _logger.LogInformationString($"Got {noteInfo.Length} notes info");
                    var client = _analyzerServiceClient.AnalyzeBulk();
                    await client.RequestStream.WriteAllAsync(
                        noteInfo.Select(
                            note => new AnalyzeRequest
                            {
                                Mode = AnalyzerMode.SudachiC,
                                Text = (note.Fields.GetValueOrDefault(
                                    "Reading"
                                ) ?? note.Fields["Text"]).Value
                            }
                        )
                    );
                    // client.ResponseStream.read
                    break;
            }
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
