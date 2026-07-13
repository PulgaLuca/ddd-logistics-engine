using Dhexby.LogisticsEngine.Domain.Hubs;
using Dhexby.LogisticsEngine.Domain.Routing;

namespace Dhexby.LogisticsEngine.Application.Routing;

/// <summary>
/// Risultato "percorso" restituito dai servizi di routing. Forma stabile e
/// condivisa (strategia + sequenza ordinata di nodi + costo totale): Dijkstra,
/// A* e i futuri algoritmi restituiscono tutti questo tipo, così il frontend
/// non deve cambiare al variare della strategia.
/// </summary>
public sealed record RouteResponse(
    string Strategy,
    bool Found,
    double TotalCost,
    int HopCount,
    IReadOnlyList<RouteNodeResponse> Nodes)
{
    public static RouteResponse From(string strategy, RoutePath path, IReadOnlyDictionary<Guid, Hub> hubs)
    {
        var nodes = new List<RouteNodeResponse>(path.HubIds.Count);
        for (int position = 0; position < path.HubIds.Count; position++)
        {
            nodes.Add(RouteNodeResponse.From(position + 1, hubs[path.HubIds[position]]));
        }

        return new RouteResponse(strategy, Found: true, path.TotalCost, Math.Max(nodes.Count - 1, 0), nodes);
    }
}

/// <summary>Un nodo del percorso (un hub), con la sua posizione nella sequenza.</summary>
public sealed record RouteNodeResponse(
    int Position,
    Guid HubId,
    string HubCode,
    string Name,
    double Latitude,
    double Longitude)
{
    public static RouteNodeResponse From(int position, Hub hub) => new(
        position,
        hub.Id,
        hub.Code.Value,
        hub.Name,
        hub.Coordinate.Latitude,
        hub.Coordinate.Longitude);
}
