using LogisticsEngine.Domain.Hubs.Entities;
using LogisticsEngine.Domain.Hubs.Vo;

namespace LogisticsEngine.Domain.Hubs.Services;

public interface IHubGraphDomainService
{
    HubPath? FindShortestPath(IEnumerable<Hub> hubs, IEnumerable<HubConnection> connections, Guid originId, Guid destinationId);
    IReadOnlyList<Guid> BreadthFirstSearch(IEnumerable<Hub> hubs, IEnumerable<HubConnection> connections, Guid originId);
    IReadOnlyList<Guid> DepthFirstSearch(IEnumerable<Hub> hubs, IEnumerable<HubConnection> connections, Guid originId);
}
