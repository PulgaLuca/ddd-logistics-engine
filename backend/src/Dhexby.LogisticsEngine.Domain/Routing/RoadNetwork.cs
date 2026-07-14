using Dhexby.LogisticsEngine.Domain.Hubs;

namespace Dhexby.LogisticsEngine.Domain.Routing;

/// <summary>
/// Domain service: il grafo della rete stradale. I nodi sono gli hub, gli archi
/// le strade (bidirezionali). È la struttura dati su cui operano Dijkstra, A*
/// e le analisi BFS/DFS.
/// </summary>
public sealed class RoadNetwork
{
    private readonly Dictionary<Guid, GeoCoordinate> _coordinates;
    private readonly Dictionary<Guid, List<RoadEdge>> _adjacency;

    private RoadNetwork(
        Dictionary<Guid, GeoCoordinate> coordinates,
        Dictionary<Guid, List<RoadEdge>> adjacency)
    {
        _coordinates = coordinates;
        _adjacency = adjacency;
    }

    public IReadOnlyCollection<Guid> Nodes => _adjacency.Keys;

    /// <summary>Costruisce il grafo dagli hub (nodi) e dalle strade (archi bidirezionali).</summary>
    public static RoadNetwork Build(IEnumerable<Hub> hubs, IEnumerable<Road> roads)
    {
        ArgumentNullException.ThrowIfNull(hubs);
        ArgumentNullException.ThrowIfNull(roads);

        var coordinates = new Dictionary<Guid, GeoCoordinate>();
        var adjacency = new Dictionary<Guid, List<RoadEdge>>();

        foreach (Hub hub in hubs)
        {
            coordinates[hub.Id] = hub.Coordinate;
            adjacency[hub.Id] = [];
        }

        foreach (Road road in roads)
        {
            if (!adjacency.TryGetValue(road.FromHubId, out List<RoadEdge>? fromEdges) ||
                !adjacency.TryGetValue(road.ToHubId, out List<RoadEdge>? toEdges))
            {
                // Ignora le strade che puntano a hub non presenti nella rete.
                continue;
            }

            fromEdges.Add(new RoadEdge(road.ToHubId, road.DistanceInKilometers, road.TrafficFactor));
            toEdges.Add(new RoadEdge(road.FromHubId, road.DistanceInKilometers, road.TrafficFactor));
        }

        return new RoadNetwork(coordinates, adjacency);
    }

    public bool ContainsNode(Guid hubId) => _adjacency.ContainsKey(hubId);

    public IReadOnlyList<RoadEdge> EdgesFrom(Guid hubId) =>
        _adjacency.TryGetValue(hubId, out List<RoadEdge>? edges) ? edges : [];

    public GeoCoordinate CoordinateOf(Guid hubId) => _coordinates[hubId];
}
