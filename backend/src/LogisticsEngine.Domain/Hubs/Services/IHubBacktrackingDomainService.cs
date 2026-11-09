using LogisticsEngine.Domain.Hubs.Entities;
using LogisticsEngine.Domain.Hubs.Vo;

namespace LogisticsEngine.Domain.Hubs.Services;

public interface IHubBacktrackingDomainService
{
    IReadOnlyList<IReadOnlyList<Guid>> FindSimplePaths(
        IEnumerable<Hub> hubs,
        IEnumerable<HubConnection> connections,
        Guid originId,
        Guid destinationId,
        int maximumPaths);
}
