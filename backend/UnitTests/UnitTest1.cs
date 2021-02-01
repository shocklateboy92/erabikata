using System;
using System.Threading.Tasks;
using Erabikata;
using Erabikata.Backend;
using Erabikata.Backend.CollectionManagers;
using Erabikata.Backend.Models.Actions;
using Grpc.Core;
using Grpc.Core.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using NUnit.Framework;

namespace UnitTests
{
    public class Tests
    {
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
        public async Task LocalTest()
        {
            var client = new AnalyzerService.AnalyzerServiceClient(
                new Channel("127.0.0.1:5001", ChannelCredentials.Insecure)
            );
            var bulk = client.AnalyzeDialogBulk();
            await bulk.RequestStream.WriteAllAsync( new[]
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
                        Lines = { "わざわざ来てくれたって", "何かあんでしょ？"},
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