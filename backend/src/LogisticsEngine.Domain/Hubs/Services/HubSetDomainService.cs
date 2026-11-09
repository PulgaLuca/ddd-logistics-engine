using LogisticsEngine.Domain.Hubs.Entities;

namespace LogisticsEngine.Domain.Hubs.Services;

/// <summary>Restituisce le zone distinte usando HashSet.</summary>
public sealed class HubSetDomainService : IHubSetDomainService
{
    public IReadOnlySet<string> DistinctZones(IEnumerable<Hub> hubs) => hubs.Select(hub => hub.Zone.Code).ToHashSet(StringComparer.Ordinal);
}
