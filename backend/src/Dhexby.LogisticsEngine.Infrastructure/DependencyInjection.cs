using Dhexby.LogisticsEngine.Domain.Hubs;
using Dhexby.LogisticsEngine.Domain.Orders;
using Dhexby.LogisticsEngine.Domain.Shipments;
using Dhexby.LogisticsEngine.Domain.Zones;
using Dhexby.LogisticsEngine.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dhexby.LogisticsEngine.Infrastructure;

/// <summary>
/// Composition root del layer Infrastructure: lega i contratti del dominio
/// alle implementazioni concrete (qui lo store JSON).
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JsonStorageOptions>(
            configuration.GetSection(JsonStorageOptions.SectionName));

        services.AddSingleton<IShipmentRepository, JsonShipmentRepository>();
        services.AddSingleton<IOrderRepository, JsonOrderRepository>();
        services.AddSingleton<IHubRepository, JsonHubRepository>();
        services.AddSingleton<IZoneRepository, JsonZoneRepository>();

        return services;
    }
}
