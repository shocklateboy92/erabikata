using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Erabikata.Backend.CollectionManagers;
using Grpc.Core.Logging;
using Microsoft.Extensions.Logging;

namespace Erabikata.Backend.Managers
{
    public class WordCountsManager
    {
        private readonly ILogger<WordCountsManager> _logger;
        private readonly DialogCollectionManager _dialogCollectionManager;

        public WordCountsManager(
            ILogger<WordCountsManager> logger,
            DialogCollectionManager dialogCollectionManager)
        {
            _logger = logger;
            _dialogCollectionManager = dialogCollectionManager;
        }

        public async Task Initialize()
        {
            _logger.LogInformation("Building word ranks map");
            var wordRanks = new Dictionary<AnalyzerMode, List<DialogCollectionManager.WordRank>>();
            foreach (var mode in new[]
            {
                AnalyzerMode.SudachiA, AnalyzerMode.SudachiB, AnalyzerMode.SudachiC
            })
            {
                wordRanks.Add(mode, await _dialogCollectionManager.GetSortedWordCounts(mode));
            }

            WordRanks = wordRanks;

            WordRanksMap = WordRanks.ToDictionary(
                ranks => ranks.Key,
                ranks => ranks.Value.Select((kv, index) => (kv, index))
                    .ToDictionary(kvp => kvp.kv.BaseForm, kvp => kvp.index)
            );
        }

        public IReadOnlyDictionary<AnalyzerMode, Dictionary<string, int>> WordRanksMap
        {
            get;
            private set;
        } = new Dictionary<AnalyzerMode, Dictionary<string, int>>();

        public IReadOnlyDictionary<AnalyzerMode, List<DialogCollectionManager.WordRank>> WordRanks
        {
            get;
            private set;
        } = new Dictionary<AnalyzerMode, List<DialogCollectionManager.WordRank>>();
    }
}