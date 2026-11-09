using LogisticsEngine.Domain.Hubs;
using LogisticsEngine.Domain.Shipments;
using LogisticsEngine.Infrastructure.Hubs;
using LogisticsEngine.Infrastructure.Shipments;
using Microsoft.Extensions.DependencyInjection;

namespace LogisticsEngine.Infrastructure;

// Extension method for DI container for earch layer
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string dataDirectory)
    {
        services.AddSingleton<IHubRepository>(new JsonHubRepository(dataDirectory));
        services.AddSingleton<IHubConnectionRepository>(new JsonHubConnectionRepository(dataDirectory));
        services.AddSingleton<IShipmentRepository>(new JsonShipmentRepository(dataDirectory));
        return services;
    }
}
