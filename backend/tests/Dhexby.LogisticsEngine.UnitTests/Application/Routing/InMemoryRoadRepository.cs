using Dhexby.LogisticsEngine.Domain.Routing;

namespace Dhexby.LogisticsEngine.UnitTests.Application.Routing;

/// <summary>Fake in-memory di <see cref="IRoadRepository"/> per i test degli handler.</summary>
internal sealed class InMemoryRoadRepository : IRoadRepository
{
    private readonly List<Road> _roads = [];

    public Task<Road?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(_roads.Find(road => road.Id == id));

    public Task<bool> ExistsBetweenAsync(
        Guid firstHubId,
        Guid secondHubId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(_roads.Exists(road =>
            (road.FromHubId == firstHubId && road.ToHubId == secondHubId) ||
            (road.FromHubId == secondHubId && road.ToHubId == firstHubId)));

    public Task<IReadOnlyList<Road>> GetAllAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Road>>(_roads.ToArray());

    public Task AddAsync(Road road, CancellationToken cancellationToken = default)
    {
        _roads.Add(road);
        return Task.CompletedTask;
    }
}
