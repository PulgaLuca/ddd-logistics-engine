namespace LogisticsEngine.Domain.Hubs;

public interface IHubRepository
{
    Task<Hub?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Hub>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Hub hub, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Hub hub, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
