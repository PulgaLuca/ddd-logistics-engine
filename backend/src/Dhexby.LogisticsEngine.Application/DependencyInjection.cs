using Dhexby.LogisticsEngine.Domain.Shipments;
using Microsoft.Extensions.DependencyInjection;

namespace Dhexby.LogisticsEngine.Application;

/// <summary>
/// Composition root del layer Application: registra MediatR e i domain service.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddSingleton<IShipmentPricingService, ShipmentPricingService>();

        return services;
    }
}
