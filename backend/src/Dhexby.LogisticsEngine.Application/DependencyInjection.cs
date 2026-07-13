using Dhexby.LogisticsEngine.Application.Abstractions.Behaviors;
using Dhexby.LogisticsEngine.Domain.Shipments;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Dhexby.LogisticsEngine.Application;

/// <summary>
/// Composition root del layer Application: registra MediatR (con la pipeline
/// di validazione), i validator FluentValidation e i domain service.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

        services.AddSingleton<IShipmentPricingService, ShipmentPricingService>();

        return services;
    }
}
