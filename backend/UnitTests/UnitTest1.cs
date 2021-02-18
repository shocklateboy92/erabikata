using System;
using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Erabikata;
using Erabikata.Backend;
using Erabikata.Backend.CollectionManagers;
using Erabikata.Backend.CollectionMiddlewares;
using Erabikata.Backend.Models.Actions;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Core.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MoreLinq;
using NUnit.Framework;

namespace UnitTests
{
    public class Tests
    {
        private static readonly Channel LocalChannel = new Channel(
            "127.0.0.1:5001",
            ChannelCredentials.Insecure
        );

        private IServiceProvider _serviceProvider = null!;

        [SetUp]
        public void Setup()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
            Environment.SetEnvironmentVariable(
                "SubtitleProcessing:Input:RootDirectory",
                "/home/fernie/src/iknow/anime-subs"
            );

            var host = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>());
            _serviceProvider = host.Build().Services;
            Manager = _serviceProvider.GetRequiredService<DialogCollectionManager>();
            EngManager = _serviceProvider.GetRequiredService<EngSubCollectionManager>();
        }

        public EngSubCollectionManager EngManager { get; set; } = null!;

        private DialogCollectionManager Manager { get; set; } = null!;

        [Test]
        public async Task Test1()
        {
            await Manager.OnActivityExecuting(new BeginIngestion("a", "b"));
        }

        [Test]
        public async Task Test2()
        {
            var res = await Manager.GetSortedWordCounts(AnalyzerMode.SudachiC, new string[] { });
            Console.WriteLine(res.Take(3).ToJson());
        }

        [Test]
        public async Task Test3()
        {
            var client = new AssParserService.AssParserServiceClient(LocalChannel);
            await using var stream = File.OpenRead(
                "/home/fernie/src/iknow/anime-subs/watched/v2/Shirobako/english/17.ass"
            );
            using var request = client.ParseAss();
            int count;
            using var memory = MemoryPool<byte>.Shared.Rent(4096);
            while ((count = await stream.ReadAsync(memory.Memory)) > 0)
            {
                await request.RequestStream.WriteAsync(
                    new ParseAssRequestChunk
                    {
                        Content = ByteString.CopyFrom(memory.Memory.ToArray(), 0, count)
                    }
                );
            }

            await request.RequestStream.CompleteAsync();

            await foreach (var response in request.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine(response.Lines.Aggregate(0, (i, s) => s.Length + i));
            }
        }

        [Test]
        public async Task TestJmDict()
        {
            await using var file = new FileStream("/home/fernie/Downloads/JMdict_e", FileMode.Open);
            var doc = await XElement.LoadAsync(file, LoadOptions.None, CancellationToken.None);
            var poses = doc.Elements("entry")
                .SelectMany(
                    entry => entry.Elements("sense")
                        .SelectMany(sense => sense.Elements("pos").Select(pos => pos.Value))
                )
                .Distinct();

            var dupes = doc.Elements("entry").Where(entry => entry.Elements("k_ele").Count() > 1);

            Console.WriteLine(string.Join(", ", poses));
        }

        [Test]
        public async Task TestWordInfoParallel()
        {
            await using var file = new FileStream("/home/fernie/Downloads/JMdict_e", FileMode.Open);
            var doc = await XElement.LoadAsync(file, LoadOptions.None, CancellationToken.None);
            var words = DictionaryProviderMiddleware.ProcessDictionary(doc);
            foreach (var batch in words.Batch(100).Batch(20))
            {
                await Task.WhenAll(
                    batch.Select(
#pragma warning disable 1998
                        async (infos, count) =>
#pragma warning restore 1998
                        {
                            Console.WriteLine(
                                $"making bulk request {count} with {infos.Count()} items"
                            );
                            return Array.Empty<object>();
                        }
                    )
                );
            }

            // Console.WriteLine(words.Batch(100).Batch(20).Count());
        }

        [Test]
        public async Task TestWordMatching()
        {
            var wordCm = _serviceProvider.GetRequiredService<WordInfoCollectionManager>();
            var stopwatch = Stopwatch.StartNew();
            var words = await wordCm.GetAllWords();
            Console.WriteLine($"Reading words took {stopwatch.ElapsedMilliseconds}ms");
            stopwatch.Restart();
            await Manager.ProcessWords2(words);
            Console.WriteLine($"Processing words took {stopwatch.ElapsedMilliseconds}ms");
        }

        [Test]
        public async Task Test5()
        {
            var rest = await EngManager.GetNearestSubs(1865, 47, 3, Array.Empty<string>());
            Console.WriteLine(rest.ToJson(new JsonWriterSettings {Indent = true}));
        }

        [Test]
        public async Task TestFindNearestDialog()
        {
            var dialog = await Manager.GetNearestDialog(2056, 47, 6, AnalyzerMode.SudachiC);
            Console.WriteLine(
                dialog.Select(d => d.Time)
                    .ToArray()
                    .ToJson(new JsonWriterSettings {Indent = true})
            );
        }

        [Test]
        public async Task LocalTest()
        {
            var client = new AnalyzerService.AnalyzerServiceClient(LocalChannel);
            var bulk = client.AnalyzeDialogBulk();
            await bulk.RequestStream.WriteAllAsync(
                new[]
                {
                    new AnalyzeDialogRequest
                    {
                        Lines = {"やっぱりできた子だね", "どうするかな"},
                        Mode = AnalyzerMode.SudachiC,
                        Style = "yolo",
                        Time = 0.3
                    },
                    new AnalyzeDialogRequest
                    {
                        Lines = {"わざわざ来てくれたって", "何かあんでしょ？"},
                        Mode = AnalyzerMode.SudachiC,
                        Style = "yolo",
                        Time = 4.3
                    }
                }
            );

            await foreach (var response in bulk.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine(response.Lines);
            }
        }
    }
}