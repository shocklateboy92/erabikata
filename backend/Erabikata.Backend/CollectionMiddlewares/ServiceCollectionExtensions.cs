using Microsoft.Extensions.DependencyInjection;

namespace Erabikata.Backend.CollectionMiddlewares
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCollectionMiddlewares(
            this IServiceCollection serviceCollection
        ) {
            serviceCollection.AddHttpClient<DictionaryProviderMiddleware>();
            return serviceCollection.AddSingleton<
                ICollectionMiddleware,
                RevisionControlMiddleware
            >()
                .AddSingleton<ICollectionMiddleware, DictionaryProviderMiddleware>()
                .AddSingleton<ICollectionMiddleware, WordInfoNormalizeMiddleware>()
                .AddSingleton<ICollectionMiddleware, DialogPostprocessingMiddleware>()
                .AddSingleton<ICollectionMiddleware, AnkiConnectMiddleware>()
                .AddSingleton<ICollectionMiddleware, SeedDataMiddleware>();
        }
    }
}
