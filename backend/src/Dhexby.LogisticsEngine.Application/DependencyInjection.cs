using Dhexby.LogisticsEngine.Application.Abstractions.Behaviors;
using Dhexby.LogisticsEngine.Application.Routing;
using Dhexby.LogisticsEngine.Domain.Orders.Dispatching;
using Dhexby.LogisticsEngine.Domain.Orders.Processing;
using Dhexby.LogisticsEngine.Domain.Routing;
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

            // L'ordine di registrazione è l'ordine di esecuzione: prima si valida
            // l'input (niente transazione per una richiesta malformata), poi si
            // apre il confine transazionale attorno all'handler.
            configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
            configuration.AddOpenBehavior(typeof(TransactionBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

        services.AddSingleton<IShipmentPricingService, ShipmentPricingService>();

        // Strutture dati di dominio esposte come servizi.
        // La cronologia di dispatch è un singleton stateful (mantiene lo stato
        // undo/redo per il tempo di vita del processo); le code sono stateless.
        services.AddSingleton<IDispatchHistory, DispatchHistoryStack>();
        services.AddSingleton<IOrderProcessingQueue, OrderProcessingQueue>();
        services.AddSingleton<IOrderDispatchPriorityQueue, OrderDispatchPriorityQueue>();

        // Servizi di routing/analisi di rete (stateless). Le due strategie di
        // percorso sono registrate come tipo concreto perché gli handler ne
        // scelgono una specifica (Dijkstra vs A*), pur condividendo IRouteCalculationService.
        services.AddSingleton<DijkstraRouteCalculationService>();
        services.AddSingleton<SmartRoutePlanner>();
        services.AddSingleton<INetworkAnalysisService, NetworkAnalysisService>();
        services.AddScoped<IRoadNetworkFactory, RoadNetworkFactory>();

        return services;
    }
}
