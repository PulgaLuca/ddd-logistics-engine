using LogisticsEngine.Domain.Hubs.Entities;

namespace LogisticsEngine.Domain.Hubs.Services;

public interface IHubTreeDomainService
{
    IReadOnlyList<Hub> TraverseInOrder(IEnumerable<Hub> hubs);
}
