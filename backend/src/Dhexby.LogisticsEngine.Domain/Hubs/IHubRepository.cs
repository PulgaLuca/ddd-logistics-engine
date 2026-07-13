namespace Dhexby.LogisticsEngine.Domain.Hubs;

/// <summary>
/// Porta di persistenza dell'aggregato Hub. <see cref="GetAllAsync"/> è pensato
/// per alimentare in futuro la costruzione del grafo della rete stradale
/// (nodi = hub) usata da RoadNetwork e dagli algoritmi di routing.
/// </summary>
public interface IHubRepository
{
    Task<Hub?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(HubCode code, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Hub>> GetAllAsync(CancellationToken cancellationToken = default);

    Task AddAsync(Hub hub, CancellationToken cancellationToken = default);

    Task UpdateAsync(Hub hub, CancellationToken cancellationToken = default);
}
