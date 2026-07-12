using Dhexby.LogisticsEngine.Domain.Shipments;
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
        services.Configure<ShipmentStorageOptions>(
            configuration.GetSection(ShipmentStorageOptions.SectionName));

        services.AddSingleton<IShipmentRepository, JsonShipmentRepository>();

        return services;
    }
}
