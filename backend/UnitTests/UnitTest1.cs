using System;
using System.Threading.Tasks;
using Erabikata;
using Erabikata.Backend;
using Erabikata.Backend.CollectionManagers;
using Erabikata.Backend.Models.Actions;
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

        private DialogCollectionManager Manager { get; set; }

        [Test]
        public async Task Test1()
        {
            await Manager.OnActivityExecuting(new BeginIngestion("a", "b"));
        }
    }
}