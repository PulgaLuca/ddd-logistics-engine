using Dhexby.LogisticsEngine.Application.Abstractions.Behaviors;
using Dhexby.LogisticsEngine.Domain.Orders.Dispatching;
using Dhexby.LogisticsEngine.Domain.Orders.Processing;
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

        // Strutture dati di dominio esposte come servizi.
        // La cronologia di dispatch è un singleton stateful (mantiene lo stato
        // undo/redo per il tempo di vita del processo); le code sono stateless.
        services.AddSingleton<IDispatchHistory, DispatchHistoryStack>();
        services.AddSingleton<IOrderProcessingQueue, OrderProcessingQueue>();
        services.AddSingleton<IOrderDispatchPriorityQueue, OrderDispatchPriorityQueue>();

        return services;
    }
}
