using LogisticsEngine.Domain.Hubs.Entities;

namespace LogisticsEngine.Domain.Hubs.Services;

public interface IHubHashMapDomainService
{
    IReadOnlyDictionary<Guid, Hub> IndexById(IEnumerable<Hub> hubs);
}
