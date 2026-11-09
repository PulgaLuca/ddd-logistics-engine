using LogisticsEngine.Domain.Hubs.Entities;

namespace LogisticsEngine.Domain.Hubs.Services;

public interface IHubQueueDomainService
{
    IReadOnlyList<Hub> ProcessInFifoOrder(IEnumerable<Hub> hubs);
}
