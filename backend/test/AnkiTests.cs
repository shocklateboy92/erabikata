using System;
using System.Linq;
using System.Threading.Tasks;
using Erabikata.Backend.CollectionManagers;
using Erabikata.Backend.CollectionMiddlewares;
using Erabikata.Backend.Models;
using Erabikata.Tests.Generated;
using FluentAssertions;
using Moq;
using Xunit;
using Activity = Erabikata.Backend.Models.Actions.Activity;
using SendToAnki = Erabikata.Backend.Models.Actions.SendToAnki;
using SyncAnki = Erabikata.Backend.Models.Actions.SyncAnki;

namespace Erabikata.Tests;

public class AnkiTests : IClassFixture<BackendFactory>
{
    private const int TestWordId = 1414500;
    private readonly AnkiWordCollectionManager _ankiWordCollectionManager;
    private readonly BackendFactory _factory;

    public AnkiTests(AnkiWordCollectionManager ankiWordCollectionManager, BackendFactory factory)
    {
        _ankiWordCollectionManager = ankiWordCollectionManager;
        _factory = factory;
    }

    [Fact]
    public async Task TestAnkiSync()
    {
        await _ankiWordCollectionManager.OnActivityExecuting(new SyncAnki());

        var isWordInAnki = await _ankiWordCollectionManager.IsWordInAnki(1351760);
        isWordInAnki.Should().Be(true);
    }

    [Fact]
    public async Task TestSendToAnki()
    {
        var mock = new Mock<IAnkiSyncClient>();
        var target = new AnkiConnectMiddleware(mock.Object);
        mock.Setup(client => client.Execute(It.IsAny<AddNoteAnkiAction>()))
            .Returns(() => Task.FromResult(new AnkiResponse<long?>(null, "Test Error")));

        var sendToAnki = new SendToAnki(
            "Test",
            "Test meaning",
            new SendToAnki.ImageRequest("2938", 0.0),
            "Test Word",
            "Test Word Reading",
            "Test Word Meaning",
            "Test Notes",
            "https://some.link"
        );

        await target
            .Awaiting(
                t =>
                    t.Execute(
                        sendToAnki,
                        activity =>
                        {
                            activity.Should().BeEquivalentTo(sendToAnki);
                            return Task.CompletedTask;
                        }
                    )
            )
            .Should()
            .ThrowAsync<AnkiConnectException>()
            .WithMessage("Test Error");

        mock.Verify(client => client.Execute(It.IsAny<AddNoteAnkiAction>()), Times.Once);

        var generic = new Activity();
        await target.Execute(
            generic,
            activity =>
            {
                activity.Should().BeEquivalentTo(generic);
                return Task.CompletedTask;
            }
        );
    }

    [Fact]
    public async Task TestNotes()
    {
        var client = new WordsClient(_factory.CreateClient());
        var notes = await client.NotesAsync(TestWordId);
        notes
            .Should()
            .BeEquivalentTo(
                new NoteInfo[]
                {
                    new(
                        1646562351243,
                        "大胆",
                        "だいたん",
                        new[]
                        {
                            TestWordId,
                            1009470,
                            1928670,
                            2029110,
                            2089020,
                            2578080,
                            2702090,
                            1185930,
                            2028930,
                            1582920
                        },
                        words: ArraySegment<WordRef>.Empty
                    )
                },
                options => options.Excluding(info => info.Words)
            );

        string.Join(string.Empty, notes[0].Words.Select(word => word.DisplayText))
            .Should()
            .Be("この大胆な下着が？");
    }

    [Fact]
    public async Task TestKnownWords()
    {
        var client = new WordsClient(_factory.CreateClient());
        var known = await client.KnownAsync();
        known[TestWordId.ToString()].Should().BeTrue();
    }

    [Fact]
    public async Task TestKnownReadings()
    {
        const int knownWordId = 1311110; // 私
        var client = new WordsClient(_factory.CreateClient());
        var actionsClient = new ActionsClient(_factory.CreateClient());
        await actionsClient.ExecuteAsync(new UnLearnReading(knownWordId));
        
        var known = await client.WithReadingsKnownAsync();
        known.Should().NotContain(knownWordId);
        
        await actionsClient.ExecuteAsync(new LearnReading(knownWordId));
        known = await client.WithReadingsKnownAsync();
        known.Should().Contain(knownWordId);
    }
}
