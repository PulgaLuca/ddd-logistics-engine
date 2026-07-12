using Dhexby.LogisticsEngine.Domain.Shipments;

namespace Dhexby.LogisticsEngine.UnitTests.Application.Shipments;

/// <summary>
/// Fake in-memory del repository: consente di testare gli handler
/// senza toccare il file system (nessun mock framework necessario).
/// </summary>
internal sealed class InMemoryShipmentRepository : IShipmentRepository
{
    private readonly Dictionary<Guid, Shipment> _shipments = [];

    public int UpdateCallCount { get; private set; }

    public Task<Shipment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(_shipments.GetValueOrDefault(id));

    public Task<bool> ExistsByTrackingNumberAsync(
        TrackingNumber trackingNumber,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(_shipments.Values.Any(shipment => shipment.TrackingNumber.Equals(trackingNumber)));

    public Task AddAsync(Shipment shipment, CancellationToken cancellationToken = default)
    {
        _shipments.Add(shipment.Id, shipment);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Shipment shipment, CancellationToken cancellationToken = default)
    {
        UpdateCallCount++;
        _shipments[shipment.Id] = shipment;
        return Task.CompletedTask;
    }
}
