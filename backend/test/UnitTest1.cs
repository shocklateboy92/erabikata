using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Erabikata.Backend.CollectionManagers;
using Erabikata.Backend.CollectionMiddlewares;
using Erabikata.Backend.Controllers;
using Erabikata.Backend.Models.Database;
using Erabikata.Tests.Generated;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Xunit;
using Xunit.Priority;
using BeginIngestion = Erabikata.Backend.Models.Actions.BeginIngestion;
using DictionaryUpdate = Erabikata.Backend.Models.Actions.DictionaryUpdate;

namespace Erabikata.Tests;

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
        ILoggerFactory loggerFactory
    )
    {
        _factory = factory.WithWebHostBuilder(
            builder =>
                builder.ConfigureAppConfiguration(
                    (context, configurationBuilder) =>
                        configurationBuilder.AddYamlFile("testSettings.yaml")
                )
        );
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

        var words = await _wordInfoCollectionManager.GetWords(new[] { 1008050 });
        words.Single().Kanji.Should().Contain("序でに");
    }

    [Fact, Priority(2)]
    public async Task IngestSubs()
    {
        await _databaseInfoManager.OnActivityExecuting(new BeginIngestion(string.Empty, "prev"));
        var response = await _actionsController.Execute(new BeginIngestion("prev", "yolo"));
        response.Should().BeOfType<OkObjectResult>();

        var endCommit = await _databaseInfoManager.GetCurrentCommit();
        endCommit.Should().Be("yolo");
    }

    [Fact(Skip = "Manual test"), Priority(19)]
    public async Task ReprocessDialogMatches()
    {
        var middleware = new DialogPostprocessingMiddleware(
            _wordInfoCollectionManager,
            _dialogCollectionManager
        );
        await middleware.Execute(
            new BeginIngestion(string.Empty, string.Empty),
            activity => Task.CompletedTask
        );
    }

    [Theory, Priority(20)]
    [InlineData(1249960, "兄弟", 1)]
    [InlineData(1598360, "手作り", 4)]
    [InlineData(1247250, "君", 19)]
    [InlineData(1599420, "中々", 3)]
    [InlineData(1625780, "初めまして", 12)]
    [InlineData(1602440, "増える", 2)]
    [InlineData(1591330, "気づく", 3)]
    public async Task TestProcessWords2(int wordId, string text, uint count)
    {
        var words = await _wordInfoCollectionManager.GetWords(new[] { wordId });
        var wordInfo = words.Single();
        wordInfo.Kanji.First().Should().Be(text);
        wordInfo.TotalOccurrences.Should().Be(count);
    }

    [Fact]
    public async Task TestAlternateIds()
    {
        var client = new AlternateIdsClient(_factory.CreateClient());
        var map = await client.MapAsync();
        foreach (var (key, value) in map)
        {
            var keyInt = int.Parse(key);
            var valueInt = int.Parse(value);
            (valueInt - keyInt).Should().Be(500);
        }
    }

    [Fact, Priority(20)]
    public async Task TestEngSubs()
    {
        var client = new EngSubsClient(_factory.CreateClient());

        const int showId = 2934;
        var styles = await client.StylesOfAsync(showId);
        styles.ShowId.Should().Be(showId);
        styles.EnabledStyles
            .Should()
            .BeEquivalentTo("Default", "DefaultLow", "Italics", "On Top", "OS");
        styles.AllStyles
            .Should()
            .BeEquivalentTo(
                new AggregateSortByCountResultOfString[]
                {
                    new(666, "Default"),
                    new(5, "On Top"),
                    new(1, "OS")
                }
            );

        var active = await client.ActiveStylesForAsync(showId);
        active.Should().BeEquivalentTo(styles.EnabledStyles);

        var osDialog = await client.ByStyleNameAsync(showId, "OS", max: 20, skip: 0);
        const string episodeId = "2936";
        const double time = 1448.0;
        osDialog.Dialog
            .Should()
            .BeEquivalentTo(
                new[]
                {
                    new Sentence(
                        episodeId: episodeId,
                        episodeTitle: null,
                        id: string.Empty,
                        text: new[] { "That Key", "Was Never Handled", "Until Today." },
                        time: time
                    )
                },
                options => options.Excluding(sentence => sentence.Id)
            );

        var showIdResult = await client.ShowIdOfAsync(episodeId);
        showIdResult.Should().Be(showId);

        var dialog = await client.IndexAsync(episodeId, time, 3);
        dialog.Dialog[3].Should().BeEquivalentTo(osDialog.Dialog[0]);
    }
}
