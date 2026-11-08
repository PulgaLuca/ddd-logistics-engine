using LogisticsEngine.Domain.Hubs.Entities;
using LogisticsEngine.Domain.Shared.Sorting;

namespace LogisticsEngine.Domain.Hubs.Services;

/// <summary>Espone ordinamenti di dominio per le chiavi disponibili su Hub.</summary>
public sealed class HubSortingDomainService(ISorter<Hub> sorter) : IHubSortingDomainService
{
    public IReadOnlyList<Hub> SortById(IEnumerable<Hub> hubs) => sorter.Sort(hubs, hub => hub.Id);
    public IReadOnlyList<Hub> SortByName(IEnumerable<Hub> hubs) => sorter.Sort(hubs, hub => hub.Name.Value, StringComparer.Ordinal);
    public IReadOnlyList<Hub> SortByZone(IEnumerable<Hub> hubs) => sorter.Sort(hubs, hub => hub.Zone.Code, StringComparer.Ordinal);
}
