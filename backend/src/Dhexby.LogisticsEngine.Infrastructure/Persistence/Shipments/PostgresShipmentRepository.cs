using Dhexby.LogisticsEngine.Domain.Shipments;
using Microsoft.EntityFrameworkCore;

namespace Dhexby.LogisticsEngine.Infrastructure.Persistence;

/// <summary>
/// Repository di Shipment su PostgreSQL (tabella "shipments").
/// <para>
/// A differenza dei provider Json e Mongo, qui <see cref="AddAsync"/> e
/// <see cref="UpdateAsync"/> non scrivono subito: registrano l'intenzione nel
/// change tracker. È l'Unit of Work, al termine del comando, a fare
/// SaveChanges dentro la transazione (transactional pattern).
/// </para>
/// </summary>
internal sealed class PostgresShipmentRepository : IShipmentRepository
{
    private readonly LogisticsDbContext _context;

    public PostgresShipmentRepository(LogisticsDbContext context)
    {
        _context = context;
    }

    public async Task<Shipment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // AsNoTracking: l'aggregato viene ricostruito dal documento, quindi
        // l'istanza tracciata non servirebbe e creerebbe solo conflitti di
        // identità al successivo Update.
        ShipmentDocument? document = await _context.Shipments
            .AsNoTracking()
            .FirstOrDefaultAsync(document => document.Id == id, cancellationToken);

        return document?.ToDomain();
    }

    public Task<bool> ExistsByTrackingNumberAsync(
        TrackingNumber trackingNumber,
        CancellationToken cancellationToken = default) =>
        _context.Shipments
            .AsNoTracking()
            .AnyAsync(document => document.TrackingNumber == trackingNumber.Value, cancellationToken);

    public async Task AddAsync(Shipment shipment, CancellationToken cancellationToken = default) =>
        await _context.Shipments.AddAsync(ShipmentDocument.FromDomain(shipment), cancellationToken);

    public async Task UpdateAsync(Shipment shipment, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(shipment);

        bool exists = await _context.Shipments
            .AsNoTracking()
            .AnyAsync(document => document.Id == shipment.Id, cancellationToken);

        if (!exists)
        {
            throw new InvalidOperationException(
                $"Cannot update shipment '{shipment.Id}': not found in PostgreSQL.");
        }

        _context.Shipments.Update(ShipmentDocument.FromDomain(shipment));
    }
}
