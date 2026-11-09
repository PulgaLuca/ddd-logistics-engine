using LogisticsEngine.Domain.Hubs.Entities;

namespace LogisticsEngine.Domain.Hubs.Services;

/// <summary>Indicizza gli Hub per ID usando Dictionary&lt;TKey,TValue&gt;.</summary>
public sealed class HubHashMapDomainService : IHubHashMapDomainService
{
    public IReadOnlyDictionary<Guid, Hub> IndexById(IEnumerable<Hub> hubs) => hubs.ToDictionary(hub => hub.Id);
}
