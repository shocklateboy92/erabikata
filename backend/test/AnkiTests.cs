using System.Threading.Tasks;
using Erabikata.Backend.CollectionManagers;
using Erabikata.Backend.CollectionMiddlewares;
using Erabikata.Backend.Models;
using Erabikata.Backend.Models.Actions;
using FluentAssertions;
using Moq;
using Xunit;

namespace Erabikata.Tests;

public class AnkiTests 
{
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
}
