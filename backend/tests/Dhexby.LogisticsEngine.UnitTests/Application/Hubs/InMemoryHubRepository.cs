using Dhexby.LogisticsEngine.Domain.Hubs;

namespace Dhexby.LogisticsEngine.UnitTests.Application.Hubs;

/// <summary>Fake in-memory di <see cref="IHubRepository"/> per i test degli handler.</summary>
internal sealed class InMemoryHubRepository : IHubRepository
{
    private readonly Dictionary<Guid, Hub> _hubs = [];

    public Task<Hub?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(_hubs.GetValueOrDefault(id));

    public Task<bool> ExistsByCodeAsync(HubCode code, CancellationToken cancellationToken = default) =>
        Task.FromResult(_hubs.Values.Any(hub => hub.Code.Equals(code)));

    public Task<IReadOnlyList<Hub>> GetAllAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Hub>>(_hubs.Values.ToArray());

    public Task AddAsync(Hub hub, CancellationToken cancellationToken = default)
    {
        _hubs.Add(hub.Id, hub);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Hub hub, CancellationToken cancellationToken = default)
    {
        _hubs[hub.Id] = hub;
        return Task.CompletedTask;
    }
}
