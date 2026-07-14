namespace Dhexby.LogisticsEngine.Domain.Routing;

/// <summary>
/// Esito dell'analisi di raggiungibilità: se la destinazione è raggiungibile,
/// il percorso a minor numero di salti (BFS) e gli ordini di visita BFS e DFS.
/// </summary>
public sealed record ReachabilityResult(
    bool CanReach,
    IReadOnlyList<Guid> Path,
    IReadOnlyList<Guid> BreadthFirstOrder,
    IReadOnlyList<Guid> DepthFirstOrder);
