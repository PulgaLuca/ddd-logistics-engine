namespace Dhexby.LogisticsEngine.Domain.Routing;

/// <summary>Porta di persistenza degli archi (strade) della rete.</summary>
public interface IRoadRepository
{
    Task<Road?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> ExistsBetweenAsync(Guid firstHubId, Guid secondHubId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Road>> GetAllAsync(CancellationToken cancellationToken = default);

    Task AddAsync(Road road, CancellationToken cancellationToken = default);
}
