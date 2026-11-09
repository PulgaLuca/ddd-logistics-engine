using LogisticsEngine.Domain.Hubs.Entities;
using LogisticsEngine.Domain.Hubs.Vo;

namespace LogisticsEngine.Domain.Hubs.Services;

/// <summary>Algoritmi su un grafo non orientato e pesato di Hub.</summary>
public sealed class HubGraphDomainService : IHubGraphDomainService
{
    public HubPath? FindShortestPath(IEnumerable<Hub> hubs, IEnumerable<HubConnection> connections, Guid originId, Guid destinationId)
    {
        Dictionary<Guid, List<Neighbor>> graph = BuildGraph(hubs, connections);
        if (!graph.ContainsKey(originId) || !graph.ContainsKey(destinationId))
        {
            return null;
        }

        Dictionary<Guid, decimal> distances = graph.Keys.ToDictionary(id => id, _ => decimal.MaxValue);
        Dictionary<Guid, Guid> previous = new();
        PriorityQueue<Guid, decimal> pending = new();
        distances[originId] = 0;
        pending.Enqueue(originId, 0);

        while (pending.TryDequeue(out Guid currentId, out decimal distance))
        {
            if (distance > distances[currentId])
            {
                continue;
            }

            if (currentId == destinationId)
            {
                return BuildPath(previous, originId, destinationId, distance);
            }

            foreach (Neighbor neighbor in graph[currentId])
            {
                decimal candidateDistance = distance + neighbor.Distance;
                if (candidateDistance >= distances[neighbor.HubId])
                {
                    continue;
                }

                distances[neighbor.HubId] = candidateDistance;
                previous[neighbor.HubId] = currentId;
                pending.Enqueue(neighbor.HubId, candidateDistance);
            }
        }

        return null;
    }

    public IReadOnlyList<Guid> BreadthFirstSearch(IEnumerable<Hub> hubs, IEnumerable<HubConnection> connections, Guid originId)
    {
        Dictionary<Guid, List<Neighbor>> graph = BuildGraph(hubs, connections);
        if (!graph.ContainsKey(originId))
        {
            return [];
        }

        Queue<Guid> pending = new();
        HashSet<Guid> visited = new() { originId };
        List<Guid> result = [];
        pending.Enqueue(originId);

        while (pending.TryDequeue(out Guid currentId))
        {
            result.Add(currentId);
            foreach (Neighbor neighbor in graph[currentId])
            {
                if (visited.Add(neighbor.HubId))
                {
                    pending.Enqueue(neighbor.HubId);
                }
            }
        }

        return result;
    }

    public IReadOnlyList<Guid> DepthFirstSearch(IEnumerable<Hub> hubs, IEnumerable<HubConnection> connections, Guid originId)
    {
        Dictionary<Guid, List<Neighbor>> graph = BuildGraph(hubs, connections);
        if (!graph.ContainsKey(originId))
        {
            return [];
        }

        Stack<Guid> pending = new();
        HashSet<Guid> visited = new();
        List<Guid> result = [];
        pending.Push(originId);

        while (pending.TryPop(out Guid currentId))
        {
            if (!visited.Add(currentId))
            {
                continue;
            }

            result.Add(currentId);
            foreach (Neighbor neighbor in graph[currentId].AsEnumerable().Reverse())
            {
                if (!visited.Contains(neighbor.HubId))
                {
                    pending.Push(neighbor.HubId);
                }
            }
        }

        return result;
    }

    internal static Dictionary<Guid, List<Neighbor>> BuildGraph(IEnumerable<Hub> hubs, IEnumerable<HubConnection> connections)
    {
        Dictionary<Guid, List<Neighbor>> graph = hubs.ToDictionary(hub => hub.Id, _ => new List<Neighbor>());
        foreach (HubConnection connection in connections)
        {
            if (!graph.TryGetValue(connection.FirstHubId, out List<Neighbor>? firstNeighbors)
                || !graph.TryGetValue(connection.SecondHubId, out List<Neighbor>? secondNeighbors))
            {
                continue;
            }

            firstNeighbors.Add(new Neighbor(connection.SecondHubId, connection.Distance));
            secondNeighbors.Add(new Neighbor(connection.FirstHubId, connection.Distance));
        }

        foreach (List<Neighbor> neighbors in graph.Values)
        {
            neighbors.Sort((left, right) => left.HubId.CompareTo(right.HubId));
        }

        return graph;
    }

    private static HubPath BuildPath(Dictionary<Guid, Guid> previous, Guid originId, Guid destinationId, decimal distance)
    {
        List<Guid> path = [destinationId];
        Guid currentId = destinationId;
        while (currentId != originId)
        {
            currentId = previous[currentId];
            path.Add(currentId);
        }

        path.Reverse();
        return new HubPath(path, distance);
    }

    internal sealed record Neighbor(Guid HubId, decimal Distance);
}
