using LogisticsEngine.Domain.Hubs.Entities;

namespace LogisticsEngine.Domain.Hubs.Services;

public interface IHubTrieDomainService
{
    IReadOnlyList<Hub> SearchByNamePrefix(IEnumerable<Hub> hubs, string? prefix);
}
