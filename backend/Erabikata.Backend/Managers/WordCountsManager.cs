using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Erabikata.Backend.CollectionManagers;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Erabikata.Backend.Managers
{
    public class WordCountsManager
    {
        private readonly DialogCollectionManager _dialogCollectionManager;
        private readonly ILogger<WordCountsManager> _logger;
        private readonly PartOfSpeechFilterCollectionManager _partOfSpeechFilter;

        public WordCountsManager(
            ILogger<WordCountsManager> logger,
            DialogCollectionManager dialogCollectionManager,
            PartOfSpeechFilterCollectionManager partOfSpeechFilter)
        {
            _logger = logger;
            _dialogCollectionManager = dialogCollectionManager;
            _partOfSpeechFilter = partOfSpeechFilter;
        }

        public IReadOnlyDictionary<AnalyzerMode, Dictionary<string, int>> WordRanksMap
        {
            get;
            private set;
        } = new Dictionary<AnalyzerMode, Dictionary<string, int>>();

        public IReadOnlyDictionary<AnalyzerMode, List<AggregateSortByCountResult<string>>> WordRanks
        {
            get;
            private set;
        } = new Dictionary<AnalyzerMode, List<AggregateSortByCountResult<string>>>();

        public async Task Initialize()
        {
            _logger.LogInformation("Building word ranks map");
            var wordRanks =
                new Dictionary<AnalyzerMode, List<AggregateSortByCountResult<string>>>();
            var ignoredPartsOfSpeech = await _partOfSpeechFilter.GetIgnoredPartOfSpeech();
            foreach (var mode in new[]
            {
                AnalyzerMode.SudachiA, AnalyzerMode.SudachiB, AnalyzerMode.SudachiC
            })
                wordRanks.Add(
                    mode,
                    await _dialogCollectionManager.GetSortedWordCounts(mode, ignoredPartsOfSpeech)
                );

            WordRanks = wordRanks;

            WordRanksMap = WordRanks.ToDictionary(
                ranks => ranks.Key,
                ranks => ranks.Value.Select((kv, index) => (kv, index))
                    .ToDictionary(kvp => kvp.kv.Id, kvp => kvp.index)
            );
        }
    }
}