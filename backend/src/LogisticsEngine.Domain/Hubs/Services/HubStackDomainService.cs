using LogisticsEngine.Domain.Hubs.Entities;

namespace LogisticsEngine.Domain.Hubs.Services;

/// <summary>Estrae gli Hub in ordine LIFO usando Stack&lt;T&gt;.</summary>
public sealed class HubStackDomainService : IHubStackDomainService
{
    public IReadOnlyList<Hub> PopInLifoOrder(IEnumerable<Hub> hubs)
    {
        Stack<Hub> stack = new(hubs);
        List<Hub> popped = new(stack.Count);
        while (stack.TryPop(out Hub? hub))
        {
            popped.Add(hub);
        }

        return popped;
    }
}
