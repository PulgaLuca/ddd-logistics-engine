using LogisticsEngine.Domain.Hubs.Entities;
using LogisticsEngine.Domain.Hubs.Vo;

namespace LogisticsEngine.Domain.Hubs.Services;

/// <summary>Esplora con backtracking tutti i cammini semplici entro il limite richiesto.</summary>
public sealed class HubBacktrackingDomainService : IHubBacktrackingDomainService
{
    public IReadOnlyList<IReadOnlyList<Guid>> FindSimplePaths(
        IEnumerable<Hub> hubs,
        IEnumerable<HubConnection> connections,
        Guid originId,
        Guid destinationId,
        int maximumPaths)
    {
        if (maximumPaths <= 0)
        {
            return [];
        }

        Dictionary<Guid, List<HubGraphDomainService.Neighbor>> graph = HubGraphDomainService.BuildGraph(hubs, connections);
        if (!graph.ContainsKey(originId) || !graph.ContainsKey(destinationId))
        {
            return [];
        }

        List<IReadOnlyList<Guid>> paths = [];
        List<Guid> currentPath = [originId];
        HashSet<Guid> visited = new() { originId };
        Search(originId, destinationId, graph, maximumPaths, currentPath, visited, paths);
        return paths;
    }

    private static void Search(
        Guid currentId,
        Guid destinationId,
        IReadOnlyDictionary<Guid, List<HubGraphDomainService.Neighbor>> graph,
        int maximumPaths,
        List<Guid> currentPath,
        HashSet<Guid> visited,
        ICollection<IReadOnlyList<Guid>> paths)
    {
        if (paths.Count >= maximumPaths)
        {
            return;
        }

        if (currentId == destinationId)
        {
            paths.Add(currentPath.ToArray());
            return;
        }

        foreach (HubGraphDomainService.Neighbor neighbor in graph[currentId])
        {
            if (!visited.Add(neighbor.HubId))
            {
                continue;
            }

            currentPath.Add(neighbor.HubId);
            Search(neighbor.HubId, destinationId, graph, maximumPaths, currentPath, visited, paths);
            currentPath.RemoveAt(currentPath.Count - 1);
            visited.Remove(neighbor.HubId);

            if (paths.Count >= maximumPaths)
            {
                return;
            }
        }
    }
}
