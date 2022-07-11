using System;
using System.Linq;
using System.Threading.Tasks;
using Erabikata.Backend.CollectionManagers;
using Erabikata.Tests.Generated;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Xunit.Priority;

namespace Erabikata.Tests;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class UnitTest1 : IClassFixture<BackendFactory>
{
    private const string TestEpisodeId = "2936";
    private const int TestWordId = 1002430;
    private const int TestKnownWordId = 1414500;
    private readonly WebApplicationFactory<Backend.Startup> _factory;
    private readonly DatabaseInfoManager _databaseInfoManager;
    private readonly WordInfoCollectionManager _wordInfoCollectionManager;

    public UnitTest1(
        BackendFactory factory,
        DatabaseInfoManager databaseInfoManager,
        WordInfoCollectionManager wordInfoCollectionManager
    )
    {
        _factory = factory;
        _databaseInfoManager = databaseInfoManager;
        _wordInfoCollectionManager = wordInfoCollectionManager;
    }

    [Fact, Priority(1)]
    public async Task IngestDictionary()
    {
        var client = new ActionsClient(_factory.CreateClient());
        
        await client.ExecuteAsync(
            new DictionaryUpdate("https://public.apps.lasath.org/JMdict_e-2021-02-13.gz")
        );

        var words = await _wordInfoCollectionManager.GetWords(new[] { 1008050 });
        words.Single().Kanji.Should().Contain("序でに");
    }

    [Fact, Priority(2)]
    public async Task IngestSubs()
    {
        var client = new ActionsClient(_factory.CreateClient());
        await client.ExecuteAsync(new BeginIngestion("yolo"));

        var endCommit = await _databaseInfoManager.GetCurrentCommit();
        endCommit.Should().Be("yolo");
    }


    [Fact, Priority(4)]
    public async Task TestNotes()
    {
        var actionsClient = new ActionsClient(_factory.CreateClient());
        await actionsClient.ExecuteAsync(new SyncAnki());
        
        var client = new WordsClient(_factory.CreateClient());
        var notes = await client.NotesAsync(TestKnownWordId);
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
                            TestKnownWordId,
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

    [Fact, Priority(20)]
    public async Task TestKnownWords()
    {
        var client = new WordsClient(_factory.CreateClient());
        var known = await client.KnownAsync();
        known[TestKnownWordId.ToString()].Should().BeTrue();
    }

    [Fact, Priority(20)]
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

    [Fact, Priority(20)]
    public async Task TestAlternateIds()
    {
        var client = new AlternateIdsClient(_factory.CreateClient());
        var map = await client.MapAsync();
        foreach (var (key, value) in map)
        {
            var keyInt = int.Parse(key);
            var valueInt = int.Parse(value);
            (keyInt - valueInt).Should().Be(500);
        }
    }

    [Fact, Priority(20)]
    public async Task TestEngSubs()
    {
        var client = new EngSubsClient(_factory.CreateClient());

        const int showId = 2934;
        const string styleToToggle = "Italics";
        
        var styles = await client.StylesOfAsync(showId);
        styles.ShowId.Should().Be(showId);
        styles.EnabledStyles
            .Should()
            .BeEquivalentTo("Default", "DefaultLow", styleToToggle, "On Top", "OS");
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
        const double time = 1448.0;
        osDialog.Dialog
            .Should()
            .BeEquivalentTo(
                new[]
                {
                    new Sentence(
                        episodeId: TestEpisodeId,
                        episodeTitle: null,
                        id: string.Empty,
                        text: new[] { "That Key", "Was Never Handled", "Until Today." },
                        time: time
                    )
                },
                options => options.Excluding(sentence => sentence.Id)
            );

        var showIdResult = await client.ShowIdOfAsync(TestEpisodeId);
        showIdResult.Should().Be(showId);

        var dialog = await client.IndexAsync(TestEpisodeId, time, 3);
        dialog.Dialog[3].Should().BeEquivalentTo(osDialog.Dialog[0]);

        var actionsClient = new ActionsClient(_factory.CreateClient());
        await actionsClient.ExecuteAsync(new DisableStyle(showId, styleToToggle));
        active = await client.ActiveStylesForAsync(showId);
        active.Should().NotContain(styleToToggle);
        
        await actionsClient.ExecuteAsync(new EnableStyle(showId, styleToToggle));
        active = await client.ActiveStylesForAsync(showId);
        active.Should().Contain(styleToToggle);
    }

    [Fact, Priority(20)]
    public async Task TestEpisodeSubs()
    {
        // since Id's change each run, subs and episode controllers
        // have to be tested together to be meaningful.
        var episodeClient = new EpisodeClient(_factory.CreateClient());
        var subsClient = new SubsClient(_factory.CreateClient());

        var episode = await episodeClient.IndexAsync(TestEpisodeId);
        episode.Id.Should().Be(TestEpisodeId);
        episode.Title.Should().Be("やはり俺の青春ラブコメはまちがっている. 完 Episode 1");
        const int testIndex1 = 1;
        episode.Entries[testIndex1].Time.Should().Be(9.6);

        var dialog1 = await subsClient.ByIdAsync(episode.Entries[testIndex1].DialogId);
        dialog1.Time.Should().Be(episode.Entries[testIndex1].Time);
        dialog1.EpisodeId.Should().Be(TestEpisodeId);
        dialog1.EpisodeName.Should().Be("やはり俺の青春ラブコメはまちがっている. 完 Episode 1");
        dialog1.Text.IsSongLyric.Should().BeFalse();

        // Only testing display/original form because word parsing is covered by other tests
        string.Join(string.Empty, dialog1.Text.Words[0].Select(word => word.DisplayText))
            .Should()
            .Be("あなたの依頼が残ってる");

        const int testIndex2 = 315;
        episode.Entries[testIndex2].Time.Should().Be(1187.06);
        var dialog2 = await subsClient.ByIdAsync(episode.Entries[testIndex2].DialogId);
        string.Join(string.Empty, dialog2.Text.Words[0].Select(word => word.DisplayText))
            .Should()
            .Be("けど　家事が好きなのはホントだよ");
    }

    [Fact, Priority(20)]
    public async Task TestWords()
    {
        var client = new WordsClient(_factory.CreateClient());

        var testResults = await client.SearchAsync("お茶");
        testResults.Should().Contain(TestWordId);

        var definitions = await client.DefinitionAsync(new[] { TestWordId });
        definitions
            .Should()
            .BeEquivalentTo(
                new[]
                {
                    new WordDefinition(
                        globalRank: 34L,
                        id: TestWordId,
                        priorities: new PriorityInfo(
                            freq: false,
                            gai: false,
                            ichi: true,
                            news: true,
                            spec: false
                        ),
                        english: new EnglishWord[]
                        {
                            new(
                                senses: new[] { "tea (usu. green)" },
                                tags: new[]
                                {
                                    "noun (common) (futsuumeishi)",
                                    "polite (teineigo) language"
                                }
                            ),
                            new(
                                senses: new[] { "tea break (at work)", "tea ceremony" },
                                tags: new[] { "noun (common) (futsuumeishi)" }
                            )
                        },
                        japanese: new JapaneseWord[]
                        {
                            new(kanji: "お茶", reading: "おちゃ"),
                            new(kanji: "御茶", reading: null)
                        }
                    )
                }
            );

        var occurrences = await client.OccurrencesAsync(TestWordId);
        occurrences.WordId.Should().Be(TestWordId);
        occurrences.DialogIds.Should().HaveCount(3);

        // Since the ids are transient, we need to hydrate them
        var subsClient = new SubsClient(_factory.CreateClient());
        var dialog = await Task.WhenAll(
            occurrences.DialogIds.Select(id => subsClient.ByIdAsync(id))
        );
        dialog
            .Select(occurrence => (occurrence.EpisodeId, occurrence.Time))
            .Should()
            .BeEquivalentTo(new[] { ("1717", 1565.647), ("1717", 1748.288), ("1717", 1563.103) });
    }

    [Fact, Priority(20)]
    public async Task TestRankedWords()
    {
        var client = new WordsClient(_factory.CreateClient());
        var ranks = await client.Ranked2Async(skipKnown: false, max: 5, skip: 0);
        ranks
            .Should()
            .BeEquivalentTo(
                new WordRankInfo[]
                {
                    new(count: 488L, id: 2086960, rank: 0, text: "って"),
                    new(count: 429L, id: 2270030, rank: 1, text: "乃"),
                    new(count: 424L, id: 2702090, rank: 2, text: "ては"),
                    new(count: 419L, id: 2089020, rank: 3, text: "だ"),
                    new(count: 419L, id: 2029110, rank: 4, text: "な")
                }
            );

        var episodeRank = await client.EpisodeRankAsync(TestEpisodeId, new[] { 2270030 });
        episodeRank.Should().BeEquivalentTo(new WordRank[] { new(id: 2270030, rank: 1), });
    }

    [Fact, Priority(20)]
    public async Task TestPartOfSpeech()
    {
        var episodeClient = new EpisodeClient(_factory.CreateClient());
        var subsClient = new SubsClient(_factory.CreateClient());
        var actionsClient = new ActionsClient(_factory.CreateClient());
        
        var episode = await episodeClient.IndexAsync(TestEpisodeId);

        await actionsClient.ExecuteAsync(new IncludeReadingsOf("空白"));
        var dialog = await subsClient.ByIdAsync(episode.Entries[315].DialogId);
        dialog.Text.Words[0].First(word => word.DisplayText == "　").Reading.Should().Be("きごう");

        await actionsClient.ExecuteAsync(new IgnoreReadingsOf("空白"));
        dialog = await subsClient.ByIdAsync(episode.Entries[315].DialogId);
        dialog.Text.Words[0].First(word => word.DisplayText == "　").Reading.Should().BeEmpty();
    }
}
