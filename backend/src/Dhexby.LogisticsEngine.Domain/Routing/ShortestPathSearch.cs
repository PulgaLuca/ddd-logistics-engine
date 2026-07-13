using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Routing;

/// <summary>
/// Ricerca del cammino minimo generalizzata (DRY): con euristica nulla è
/// Dijkstra (uniform-cost), con un'euristica ammissibile è A*. Il costo degli
/// archi e l'euristica sono parametri, così un solo algoritmo serve entrambi.
/// </summary>
internal static class ShortestPathSearch
{
    public static Result<RoutePath> Find(
        RoadNetwork network,
        Guid origin,
        Guid destination,
        Func<RoadEdge, double> edgeCost,
        Func<Guid, double> heuristic)
    {
        if (!network.ContainsNode(origin))
        {
            return Result.Failure<RoutePath>(RouteErrors.OriginNotInNetwork);
        }

        if (!network.ContainsNode(destination))
        {
            return Result.Failure<RoutePath>(RouteErrors.DestinationNotInNetwork);
        }

        if (origin == destination)
        {
            return new RoutePath([origin], 0);
        }

        var gScore = new Dictionary<Guid, double> { [origin] = 0 };
        var cameFrom = new Dictionary<Guid, Guid>();
        var settled = new HashSet<Guid>();
        var open = new PriorityQueue<Guid, double>();
        open.Enqueue(origin, heuristic(origin));

        while (open.Count > 0)
        {
            Guid current = open.Dequeue();

            if (current == destination)
            {
                return new RoutePath(Reconstruct(cameFrom, current), gScore[current]);
            }

            if (!settled.Add(current))
            {
                continue;
            }

            foreach (RoadEdge edge in network.EdgesFrom(current))
            {
                double tentative = gScore[current] + edgeCost(edge);

                if (!gScore.TryGetValue(edge.ToHubId, out double best) || tentative < best)
                {
                    gScore[edge.ToHubId] = tentative;
                    cameFrom[edge.ToHubId] = current;
                    open.Enqueue(edge.ToHubId, tentative + heuristic(edge.ToHubId));
                }
            }
        }

        return Result.Failure<RoutePath>(RouteErrors.NoRouteFound);
    }

    private static List<Guid> Reconstruct(Dictionary<Guid, Guid> cameFrom, Guid current)
    {
        var path = new List<Guid> { current };

        while (cameFrom.TryGetValue(current, out Guid previous))
        {
            current = previous;
            path.Add(current);
        }

        path.Reverse();
        return path;
    }
}
