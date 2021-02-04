using Microsoft.Extensions.DependencyInjection;

namespace Erabikata.Backend.CollectionMiddlewares
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCollectionMiddlewares(
            this IServiceCollection serviceCollection) =>
            serviceCollection.AddSingleton<ICollectionMiddleware, RevisionControlMiddleware>()
                .AddSingleton<ICollectionMiddleware, SeedDataMiddleware>();
    }
}