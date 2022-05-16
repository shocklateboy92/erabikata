using System.Threading.Tasks;
using Erabikata.Backend.CollectionManagers;
using Erabikata.Backend.Models.Actions;
using FluentAssertions;
using Xunit;

namespace Erabikata.Tests;

public class AnkiTests
{
    private readonly AnkiWordCollectionManager ankiWordCollectionManager;

    public AnkiTests(AnkiWordCollectionManager ankiWordCollectionManager)
    {
        this.ankiWordCollectionManager = ankiWordCollectionManager;
    }

    [Fact]
    public async Task TestAnkiSync()
    {
        await ankiWordCollectionManager.OnActivityExecuting(new SyncAnki());

        var isWordInAnki = await ankiWordCollectionManager.IsWordInAnki(1351760);
        isWordInAnki.Should().Be(true);
    }
}
