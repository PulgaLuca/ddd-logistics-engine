using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Routing;

/// <summary>
/// Astrazione del calcolo del percorso migliore tra due hub. Consente più
/// strategie intercambiabili (Dijkstra, A*, ...) dietro lo stesso contratto.
/// </summary>
public interface IRouteCalculationService
{
    /// <summary>Nome della strategia, riportato nel risultato verso il client.</summary>
    string Strategy { get; }

    Result<RoutePath> FindRoute(RoadNetwork network, Guid originHubId, Guid destinationHubId);
}
