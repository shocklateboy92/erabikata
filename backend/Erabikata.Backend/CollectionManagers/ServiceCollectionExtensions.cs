using Microsoft.Extensions.DependencyInjection;

namespace Erabikata.Backend.CollectionManagers
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCollectionManagers(
            this IServiceCollection serviceCollection)
        {
            AddCollectionManager<WordStateManager>(serviceCollection);
            AddCollectionManager<DummyCollectionManager>(serviceCollection);
            AddCollectionManager<StyleFilterCollectionManager>(serviceCollection);
            AddCollectionManager<DialogCollectionManager>(serviceCollection);
            AddCollectionManager<PartOfSpeechFilterCollectionManager>(serviceCollection);
            AddCollectionManager<EngSubCollectionManager>(serviceCollection);
            AddCollectionManager<EpisodeInfoCollectionManager>(serviceCollection);
            AddCollectionManager<WordInfoCollectionManager>(serviceCollection);
            AddCollectionManager<DatabaseInfoManager>(serviceCollection);
            return serviceCollection;
        }

        private static void AddCollectionManager<TCollectionManager>(
            IServiceCollection serviceCollection)
            where TCollectionManager : class, ICollectionManager
        {
            serviceCollection.AddSingleton<TCollectionManager>();
            serviceCollection.AddSingleton<ICollectionManager, TCollectionManager>();
        }
    }
}