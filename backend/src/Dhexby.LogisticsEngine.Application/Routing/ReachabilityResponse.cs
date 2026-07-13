using Dhexby.LogisticsEngine.Domain.Hubs;
using Dhexby.LogisticsEngine.Domain.Routing;

namespace Dhexby.LogisticsEngine.Application.Routing;

/// <summary>
/// Risultato dell'analisi di raggiungibilità (BFS/DFS): flag di raggiungibilità,
/// percorso a minor numero di salti e ordini di visita BFS e DFS.
/// </summary>
public sealed record ReachabilityResponse(
    Guid OriginHubId,
    Guid DestinationHubId,
    bool CanReach,
    IReadOnlyList<RouteNodeResponse> Path,
    IReadOnlyList<Guid> BreadthFirstOrder,
    IReadOnlyList<Guid> DepthFirstOrder)
{
    public static ReachabilityResponse From(
        Guid originHubId,
        Guid destinationHubId,
        ReachabilityResult result,
        IReadOnlyDictionary<Guid, Hub> hubs)
    {
        var path = new List<RouteNodeResponse>(result.Path.Count);
        for (int position = 0; position < result.Path.Count; position++)
        {
            path.Add(RouteNodeResponse.From(position + 1, hubs[result.Path[position]]));
        }

        return new ReachabilityResponse(
            originHubId,
            destinationHubId,
            result.CanReach,
            path,
            result.BreadthFirstOrder,
            result.DepthFirstOrder);
    }
}
