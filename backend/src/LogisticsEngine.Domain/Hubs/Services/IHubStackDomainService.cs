using LogisticsEngine.Domain.Hubs.Entities;

namespace LogisticsEngine.Domain.Hubs.Services;

public interface IHubStackDomainService
{
    IReadOnlyList<Hub> PopInLifoOrder(IEnumerable<Hub> hubs);
}
