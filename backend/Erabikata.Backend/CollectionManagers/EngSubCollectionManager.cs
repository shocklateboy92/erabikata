using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Erabikata.Backend.DataProviders;
using Erabikata.Backend.Models.Actions;
using Erabikata.Backend.Models.Database;
using Google.Protobuf;
using Grpc.Core.Utils;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Erabikata.Backend.CollectionManagers
{
    public class EngSubCollectionManager : ICollectionManager
    {
        private readonly SeedDataProvider _seedDataProvider;
        private readonly AssParserService.AssParserServiceClient _assParserServiceClient;
        private readonly IMongoCollection<EngSub> _mongoCollection;

        public EngSubCollectionManager(
            SeedDataProvider seedDataProvider,
            IMongoDatabase mongoDatabase,
            AssParserService.AssParserServiceClient assParserServiceClient)
        {
            _seedDataProvider = seedDataProvider;
            _assParserServiceClient = assParserServiceClient;
            _mongoCollection = mongoDatabase.GetCollection<EngSub>(nameof(EngSub));
        }

        public async Task OnActivityExecuting(Activity activity)
        {
            switch (activity)
            {
                case IngestShows:
                    await _mongoCollection.DeleteManyAsync(FilterDefinition<EngSub>.Empty);
                    await IngestEngSubs();
                    break;
            }
        }

        private async Task IngestEngSubs()
        {
            var inputFiles = await _seedDataProvider.GetShowMetadataFiles("english", "ass");
            foreach (var showFiles in inputFiles)
            {
                await Task.WhenAll(showFiles.Select(IngestEpisode));
            }
        }

        private async Task IngestEpisode(SeedDataProvider.ShowContentFile showFile)
        {
            var client = _assParserServiceClient.ParseAss();
            await using var showFileStream = File.OpenRead(showFile.Path);
            using var buffer = MemoryPool<byte>.Shared.Rent(4096);
            int lastReadBytesCount;
            while ((lastReadBytesCount = await showFileStream.ReadAsync(buffer.Memory)) > 0)
            {
                await client.RequestStream.WriteAsync(
                    new ParseAssRequestChunk
                    {
                        Content = ByteString.CopyFrom(
                            buffer.Memory.ToArray(),
                            0,
                            lastReadBytesCount
                        )
                    }
                );
            }

            await client.RequestStream.CompleteAsync();

            var subtitleEvents = await client.ResponseStream.ToListAsync();
            await _mongoCollection.InsertManyAsync(
                subtitleEvents.Select(
                    dialog => new EngSub(
                        ObjectId.Empty,
                        time: TimeSpan.FromMilliseconds(dialog.Time).TotalSeconds,
                        lines: dialog.Lines.ToArray(),
                        isComment: dialog.IsComment,
                        style: dialog.Style,
                        episodeId: showFile.Id
                    )
                )
            );
        }

        public Task<List<EngSub>> GetNearestSubs(int episodeId, double time, int count) =>
            _mongoCollection.Aggregate()
                .Match(sub => sub.EpisodeId == episodeId)
                // Turns out, we can't do this using LINQ until Mongo Linq V3 gets released
                // Don't know when that will be.
                .AppendStage<EngSub>(
                    $"{{ $addFields: {{ delta: {{ $abs: {{ $subtract: ['$Time', {time}] }} }} }} }}"
                )
                .Sort("{ delta: 1 }")
                .Limit(count)
                // Undo the previous `AppendStage` so LINQ doesn't find out about it
                .AppendStage<EngSub>("{ $unset: 'delta' }")
                .SortBy(sub => sub.Time)
                .ToListAsync();
    }
}