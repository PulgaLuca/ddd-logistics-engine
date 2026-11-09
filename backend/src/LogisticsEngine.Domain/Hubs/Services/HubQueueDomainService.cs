using LogisticsEngine.Domain.Hubs.Entities;

namespace LogisticsEngine.Domain.Hubs.Services;

/// <summary>Processa gli Hub in ordine FIFO usando Queue&lt;T&gt;.</summary>
public sealed class HubQueueDomainService : IHubQueueDomainService
{
    public IReadOnlyList<Hub> ProcessInFifoOrder(IEnumerable<Hub> hubs)
    {
        Queue<Hub> queue = new(hubs);
        List<Hub> processed = new(queue.Count);
        while (queue.TryDequeue(out Hub? hub))
        {
            processed.Add(hub);
        }

        return processed;
    }
}
