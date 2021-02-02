using System;
using System.Buffers;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Erabikata;
using Erabikata.Backend;
using Erabikata.Backend.CollectionManagers;
using Erabikata.Backend.Models.Actions;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Core.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;

namespace UnitTests
{
    public class Tests
    {
        private static readonly Channel LocalChannel = new Channel(
            "127.0.0.1:5001",
            ChannelCredentials.Insecure
        );

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
            Manager = host.Build().Services.GetRequiredService<DialogCollectionManager>();
        }

        private DialogCollectionManager Manager { get; set; } = null!;

        [Test]
        public async Task Test1()
        {
            await Manager.OnActivityExecuting(new BeginIngestion("a", "b"));
        }

        [Test]
        public async Task Test2()
        {
            var res = await Manager.GetSortedWordCounts(AnalyzerMode.SudachiC);
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
            while ((count = await stream.ReadAsync(memory.Memory))> 0)
            {
                await request.RequestStream.WriteAsync(
                    new ParseAssRequestChunk {Content = ByteString.CopyFrom(memory.Memory.ToArray(), 0, count)}
                );
            }

            await request.RequestStream.CompleteAsync();

            await foreach (var response in request.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine(response.Lines.Aggregate(0, (i, s) => s.Length + i));
            }
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