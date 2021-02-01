using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Erabikata.Backend.CollectionManagers;
using Erabikata.Models.Configuration;
using Microsoft.Extensions.Options;

namespace Erabikata.Backend.Managers
{
    public class WordCountsManager
    {
        private readonly DialogCollectionManager _dialogCollectionManager;
        private readonly SubtitleProcessingSettings _settings;

        public WordCountsManager(
            IOptions<SubtitleProcessingSettings> settings,
            DialogCollectionManager dialogCollectionManager)
        {
            _settings = settings.Value;
            _dialogCollectionManager = dialogCollectionManager;
        }

        public async Task Initialize()
        {
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