using LogisticsEngine.Domain.Hubs.Entities;

namespace LogisticsEngine.Domain.Hubs.Services;

/// <summary>Contratto per l'ordinamento degli Hub in base all'identificativo.</summary>
public interface IHubSortingDomainService
{
    IReadOnlyList<Hub> SortById(IEnumerable<Hub> hubs);
    IReadOnlyList<Hub> SortByName(IEnumerable<Hub> hubs);
    IReadOnlyList<Hub> SortByZone(IEnumerable<Hub> hubs);
}
