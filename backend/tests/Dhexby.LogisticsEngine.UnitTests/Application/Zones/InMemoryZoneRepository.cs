using Dhexby.LogisticsEngine.Domain.Zones;

namespace Dhexby.LogisticsEngine.UnitTests.Application.Zones;

/// <summary>Fake in-memory di <see cref="IZoneRepository"/> per i test degli handler.</summary>
internal sealed class InMemoryZoneRepository : IZoneRepository
{
    private readonly Dictionary<Guid, Zone> _zones = [];

    public int UpdateCallCount { get; private set; }

    public Task<Zone?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(_zones.GetValueOrDefault(id));

    public Task<bool> ExistsByCodeAsync(ZoneCode code, CancellationToken cancellationToken = default) =>
        Task.FromResult(_zones.Values.Any(zone => zone.Code.Equals(code)));

    public Task<IReadOnlyList<Zone>> GetChildrenAsync(
        Guid parentZoneId,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Zone>>(
            _zones.Values.Where(zone => zone.ParentZoneId == parentZoneId).ToArray());

    public Task AddAsync(Zone zone, CancellationToken cancellationToken = default)
    {
        _zones.Add(zone.Id, zone);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Zone zone, CancellationToken cancellationToken = default)
    {
        UpdateCallCount++;
        _zones[zone.Id] = zone;
        return Task.CompletedTask;
    }
}
