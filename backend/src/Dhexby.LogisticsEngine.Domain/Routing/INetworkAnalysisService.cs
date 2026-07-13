using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Routing;

/// <summary>Analisi di raggiungibilità sulla rete tramite visite BFS/DFS.</summary>
public interface INetworkAnalysisService
{
    Result<ReachabilityResult> CheckReachability(RoadNetwork network, Guid originHubId, Guid destinationHubId);
}
