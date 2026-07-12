namespace Dhexby.LogisticsEngine.Domain.Shipments;

/// <summary>
/// Porta di persistenza dell'aggregato (Repository pattern + DIP: il contratto
/// vive nel Domain, l'implementazione concreta vive in Infrastructure).
/// </summary>
public interface IShipmentRepository
{
    Task<Shipment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> ExistsByTrackingNumberAsync(TrackingNumber trackingNumber, CancellationToken cancellationToken = default);

    Task AddAsync(Shipment shipment, CancellationToken cancellationToken = default);

    Task UpdateAsync(Shipment shipment, CancellationToken cancellationToken = default);
}
