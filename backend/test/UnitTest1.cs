using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Erabikata.Backend.CollectionManagers;
using Erabikata.Backend.CollectionMiddlewares;
using Erabikata.Backend.Controllers;
using Erabikata.Backend.Models.Actions;
using Erabikata.Backend.Models.Database;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Xunit;
using Xunit.Priority;

namespace Erabikata.Tests
{
    [TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
    public class UnitTest1 : IClassFixture<WebApplicationFactory<Backend.Startup>>
    {
        private readonly WebApplicationFactory<Backend.Startup> _factory;
        private readonly DatabaseInfoManager _databaseInfoManager;
        private readonly DialogCollectionManager _dialogCollectionManager;
        private readonly WordInfoCollectionManager _wordInfoCollectionManager;
        private readonly ActionsController _actionsController;

        public UnitTest1(
            WebApplicationFactory<Backend.Startup> factory,
            DatabaseInfoManager databaseInfoManager,
            DialogCollectionManager dialogCollectionManager,
            WordInfoCollectionManager wordInfoCollectionManager,
            IMongoCollection<ActivityExecution> mongoCollection,
            IEnumerable<ICollectionManager> collectionManagers,
            IEnumerable<ICollectionMiddleware> middlewares,
            ILoggerFactory loggerFactory)
        {
            _factory = factory;
            _actionsController = new ActionsController(
                mongoCollection,
                collectionManagers,
                loggerFactory.CreateLogger<ActionsController>(),
                middlewares
            );
            _databaseInfoManager = databaseInfoManager;
            _dialogCollectionManager = dialogCollectionManager;
            _wordInfoCollectionManager = wordInfoCollectionManager;
        }

        [Fact, Priority(1)]
        public async Task IngestDictionary()
        {
            await _actionsController.Execute(
                new DictionaryUpdate("https://public.apps.lasath.org/JMdict_e-2021-02-13.gz")
            );

            var words = await _wordInfoCollectionManager.GetWords(new[] {1008050});
            words.Single().Kanji.Should().Contain("序でに");
        }

        [Fact, Priority(2)]
        public async Task IngestSubs()
        {
            await _databaseInfoManager.OnActivityExecuting(
                new BeginIngestion(string.Empty, "prev")
            );
            var response = await _actionsController.Execute(new BeginIngestion("prev", "yolo"));
            response.Should().BeOfType<OkObjectResult>();
            var client = _factory.CreateDefaultClient();

            var endCommit = await _databaseInfoManager.GetCurrentCommit();
            endCommit.Should().Be("yolo");
        }

        [Theory]
        [InlineData(2270030, "乃", 491)]
        [InlineData(1249960, "兄弟", 1)]
        [InlineData(1598360, "手作り", 4)]
        [InlineData(1247250, "君", 19)]
        [InlineData(1599420, "中々", 3)]
        [InlineData(1625780, "初めまして", 12)]
        [InlineData(1602440, "増える", 2)]
        [InlineData(1591330, "気づく", 3)]
        public async Task TestProcessWords2(int wordId, string text, uint count)
        {
            // var middleware = new DialogPostprocessingMiddleware(
            //     _wordInfoCollectionManager,
            //     _dialogCollectionManager
            // );
            // await middleware.Execute(
            //     new BeginIngestion(string.Empty, string.Empty),
            //     activity => Task.CompletedTask
            // );

            var words = await _wordInfoCollectionManager.GetWords(new[] {wordId});
            var wordInfo = words.Single();
            wordInfo.Kanji.First().Should().Be(text);
            wordInfo.TotalOccurrences.Should().Be(count);
        }
    }
}