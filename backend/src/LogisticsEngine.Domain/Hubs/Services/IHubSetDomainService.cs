using LogisticsEngine.Domain.Hubs.Entities;

namespace LogisticsEngine.Domain.Hubs.Services;

public interface IHubSetDomainService
{
    IReadOnlySet<string> DistinctZones(IEnumerable<Hub> hubs);
}
