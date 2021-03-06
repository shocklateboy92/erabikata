using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Erabikata.Backend.DataProviders;
using Erabikata.Backend.Models.Actions;
using Erabikata.Backend.Models.Database;
using Erabikata.Backend.Models.Output;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Core.Utils;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using TaskTupleAwaiter;

namespace Erabikata.Backend.CollectionManagers
{
    public class EngSubCollectionManager : ICollectionManager
    {
        private readonly AssParserService.AssParserServiceClient _assParserServiceClient;
        private readonly ILogger<EngSubCollectionManager> _logger;
        private readonly IMongoCollection<EngSub> _mongoCollection;

        public EngSubCollectionManager(
            IMongoDatabase mongoDatabase,
            AssParserService.AssParserServiceClient assParserServiceClient,
            ILogger<EngSubCollectionManager> logger)
        {
            _assParserServiceClient = assParserServiceClient;
            _logger = logger;
            _mongoCollection = mongoDatabase.GetCollection<EngSub>(nameof(EngSub));
        }

        public async Task OnActivityExecuting(Activity activity)
        {
            switch (activity)
            {
                case IngestShows ingestShows:
                    await _mongoCollection.DeleteManyAsync(FilterDefinition<EngSub>.Empty);
                    await IngestEngSubs(ingestShows.ShowsToIngest);
                    break;
            }
        }

        private async Task IngestEngSubs(IEnumerable<IngestShows.ShowToIngest> showsToIngest)
        {
            foreach (var (files, showInfo) in showsToIngest)
                await Task.WhenAll(
                    showInfo.Episodes[0]
                        .Select(
                            (episode, index) =>
                            {
                                var epNum = index + 1;
                                var epFile = files.FirstOrDefault(
                                    filePath => SeedDataProvider.IsPathForEpisode(
                                        filePath,
                                        "english",
                                        epNum
                                    )
                                );

                                if (string.IsNullOrEmpty(epFile))
                                {
                                    _logger.LogError(
                                        "Unable to find english file for episode ({EpNum}, {Key})",
                                        epNum.ToString(),
                                        episode.Key
                                    );
                                    return Task.CompletedTask;
                                }

                                return IngestEpisode(
                                    epFile,
                                    int.Parse(episode.Key.Split('/').Last())
                                );
                            }
                        )
                );
        }

        private async Task IngestEpisode(string filePath, int id)
        {
            var client = _assParserServiceClient.ParseAss();
            await WriteFileToParserClient(client, filePath);

            var subtitleEvents = await client.ResponseStream.ToListAsync();
            await _mongoCollection.InsertManyAsync(
                subtitleEvents.Select(
                    dialog => new EngSub(
                        ObjectId.Empty,
                        time: dialog.Time,
                        lines: dialog.Lines.ToArray(),
                        isComment: dialog.IsComment,
                        style: dialog.Style,
                        episodeId: id
                    )
                ),
                new InsertManyOptions {IsOrdered = false}
            );
        }

        public static async Task WriteFileToParserClient(
            AsyncDuplexStreamingCall<ParseAssRequestChunk, AssParsedResponseDialog> client,
            string filePath)
        {
            await using var showFileStream = File.OpenRead(filePath);
            using var buffer = MemoryPool<byte>.Shared.Rent(4096);
            int lastReadBytesCount;
            while ((lastReadBytesCount = await showFileStream.ReadAsync(buffer.Memory)) > 0)
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

            await client.RequestStream.CompleteAsync();
        }

        public async Task<IEnumerable<EngSub>> GetNearestSubs(
            int episodeId,
            double time,
            int count,
            IEnumerable<string> styleFilter)
        {
            var (matchAndBefore, afterMatch) = await (
                _mongoCollection
                    .Find(
                        sub => sub.EpisodeId == episodeId && !sub.IsComment &&
                               styleFilter.Contains(sub.Style) && sub.Time <= time
                    )
                    .SortByDescending(sub => sub.Time)
                    .Limit(count + 1)
                    .ToListAsync(),
                _mongoCollection
                    .Find(
                        sub => sub.EpisodeId == episodeId && !sub.IsComment &&
                               styleFilter.Contains(sub.Style) && sub.Time > time
                    )
                    .SortBy(sub => sub.Time)
                    .Limit(count)
                    .ToListAsync());

            matchAndBefore.Reverse();
            return matchAndBefore.Concat(afterMatch);
        }

        public async Task<List<AggregateSortByCountResult<string>>> GetAllStylesOf(
            IEnumerable<int> episodeIds)
        {
            var cursor = _mongoCollection.Aggregate()
                .Match(sub => episodeIds.Contains(sub.EpisodeId))
                .SortByCount(sub => sub.Style);

            return await cursor.ToListAsync();
        }

        public Task<List<EngSub>> GetByStyleName(
            IEnumerable<int> episodeIds,
            string styleName,
            PagingInfo pagingInfo)
        {
            return _mongoCollection
                .Find(sub => episodeIds.Contains(sub.EpisodeId) && sub.Style == styleName)
                .Skip(pagingInfo.Skip)
                .Limit(pagingInfo.Max)
                .ToListAsync();
        }
    }
}