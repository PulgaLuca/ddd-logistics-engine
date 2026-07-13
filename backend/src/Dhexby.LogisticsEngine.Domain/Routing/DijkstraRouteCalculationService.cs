using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Routing;

/// <summary>
/// Dijkstra: percorso più corto per distanza fisica (ignora il traffico).
/// Euristica nulla -&gt; ricerca a costo uniforme.
/// </summary>
public sealed class DijkstraRouteCalculationService : IRouteCalculationService
{
    public string Strategy => "Dijkstra";

    public Result<RoutePath> FindRoute(RoadNetwork network, Guid originHubId, Guid destinationHubId)
    {
        ArgumentNullException.ThrowIfNull(network);

        return ShortestPathSearch.Find(
            network,
            originHubId,
            destinationHubId,
            edgeCost: edge => edge.DistanceInKilometers,
            heuristic: _ => 0);
    }
}
