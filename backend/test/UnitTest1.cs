using System.Collections.Generic;
using System.Net.Http.Json;
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

namespace Erabikata.Tests
{
    public class UnitTest1 : IClassFixture<WebApplicationFactory<Erabikata.Backend.Startup>>
    {
        private readonly WebApplicationFactory<Backend.Startup> _factory;
        private readonly DatabaseInfoManager _databaseInfoManager;
        private readonly ActionsController _actionsController;

        public UnitTest1(
            WebApplicationFactory<Erabikata.Backend.Startup> factory,
            DatabaseInfoManager databaseInfoManager,
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
        }

        [Fact]
        public async Task Test1()
        {
            var response = await _actionsController.Execute(new BeginIngestion(string.Empty, "yolo"));
            response.Should().BeOfType<OkResult>();
            var client = _factory.CreateDefaultClient();

            var endCommit = await _databaseInfoManager.GetCurrentCommit();
            endCommit.Should().Be("yolo");
        }
    }
}