using Dhexby.LogisticsEngine.Domain.Hubs;
using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Routing;

/// <summary>
/// A*: routing "smart" che pesa gli archi per il traffico (distanza x fattore)
/// e usa come euristica la distanza in linea d'aria verso la destinazione.
/// L'euristica è ammissibile: la distanza geodetica non supera mai il costo
/// reale (distanza x traffico, con traffico &gt;= 1), quindi il percorso è ottimo.
/// </summary>
public sealed class SmartRoutePlanner : IRouteCalculationService
{
    public string Strategy => "AStar";

    public Result<RoutePath> FindRoute(RoadNetwork network, Guid originHubId, Guid destinationHubId)
    {
        ArgumentNullException.ThrowIfNull(network);

        if (!network.ContainsNode(destinationHubId))
        {
            return Result.Failure<RoutePath>(RouteErrors.DestinationNotInNetwork);
        }

        GeoCoordinate destination = network.CoordinateOf(destinationHubId);

        return ShortestPathSearch.Find(
            network,
            originHubId,
            destinationHubId,
            edgeCost: edge => edge.SmartCost,
            heuristic: hubId => network.CoordinateOf(hubId).DistanceInKilometersTo(destination));
    }
}
