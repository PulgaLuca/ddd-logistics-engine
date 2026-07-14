using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Routing;

/// <summary>
/// Domain service di raggiungibilità: risponde a "posso raggiungere questo hub?"
/// tramite una visita in ampiezza (BFS, che fornisce anche il percorso a minor
/// numero di salti) affiancata da una visita in profondità (DFS).
/// </summary>
public sealed class NetworkAnalysisService : INetworkAnalysisService
{
    public Result<ReachabilityResult> CheckReachability(
        RoadNetwork network,
        Guid originHubId,
        Guid destinationHubId)
    {
        ArgumentNullException.ThrowIfNull(network);

        if (!network.ContainsNode(originHubId))
        {
            return Result.Failure<ReachabilityResult>(RouteErrors.OriginNotInNetwork);
        }

        if (!network.ContainsNode(destinationHubId))
        {
            return Result.Failure<ReachabilityResult>(RouteErrors.DestinationNotInNetwork);
        }

        (IReadOnlyList<Guid> breadthOrder, Dictionary<Guid, Guid> cameFrom) = BreadthFirst(network, originHubId);
        IReadOnlyList<Guid> depthOrder = DepthFirst(network, originHubId);

        bool canReach = originHubId == destinationHubId || cameFrom.ContainsKey(destinationHubId);
        IReadOnlyList<Guid> path = canReach
            ? Reconstruct(cameFrom, originHubId, destinationHubId)
            : [];

        return new ReachabilityResult(canReach, path, breadthOrder, depthOrder);
    }

    private static (IReadOnlyList<Guid> Order, Dictionary<Guid, Guid> CameFrom) BreadthFirst(
        RoadNetwork network,
        Guid origin)
    {
        var order = new List<Guid>();
        var cameFrom = new Dictionary<Guid, Guid>();
        var visited = new HashSet<Guid> { origin };
        var queue = new Queue<Guid>();
        queue.Enqueue(origin);

        while (queue.Count > 0)
        {
            Guid current = queue.Dequeue();
            order.Add(current);

            foreach (RoadEdge edge in network.EdgesFrom(current))
            {
                if (visited.Add(edge.ToHubId))
                {
                    cameFrom[edge.ToHubId] = current;
                    queue.Enqueue(edge.ToHubId);
                }
            }
        }

        return (order, cameFrom);
    }

    private static List<Guid> DepthFirst(RoadNetwork network, Guid origin)
    {
        var order = new List<Guid>();
        var visited = new HashSet<Guid>();
        var stack = new Stack<Guid>();
        stack.Push(origin);

        while (stack.Count > 0)
        {
            Guid current = stack.Pop();
            if (!visited.Add(current))
            {
                continue;
            }

            order.Add(current);

            // Impila i vicini in ordine inverso per visitarli in ordine naturale.
            IReadOnlyList<RoadEdge> edges = network.EdgesFrom(current);
            for (int i = edges.Count - 1; i >= 0; i--)
            {
                if (!visited.Contains(edges[i].ToHubId))
                {
                    stack.Push(edges[i].ToHubId);
                }
            }
        }

        return order;
    }

    private static List<Guid> Reconstruct(Dictionary<Guid, Guid> cameFrom, Guid origin, Guid destination)
    {
        var path = new List<Guid> { destination };
        Guid current = destination;

        while (current != origin && cameFrom.TryGetValue(current, out Guid previous))
        {
            current = previous;
            path.Add(current);
        }

        path.Reverse();
        return path;
    }
}
