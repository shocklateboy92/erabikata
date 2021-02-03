using System.Buffers;
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
using MoreLinq;

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
                case BeginIngestion:
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
                await Task.WhenAll(
                    showFiles.Select(
                        async showFile =>
                        {
                            var client = _assParserServiceClient.ParseAss();
                            await using var showFileStream = File.OpenRead(showFile.Path);
                            using var buffer = MemoryPool<byte>.Shared.Rent(4096);
                            int lastReadBytesCount;
                            while ((lastReadBytesCount =
                                await showFileStream.ReadAsync(buffer.Memory)) > 0)
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
                                        time: dialog.Time,
                                        lines: dialog.Lines.ToArray(),
                                        isComment: dialog.IsComment,
                                        style: dialog.Style,
                                        episodeId: showFile.Id
                                    )
                                )
                            );
                        }
                    )
                );
            }
        }
    }
}