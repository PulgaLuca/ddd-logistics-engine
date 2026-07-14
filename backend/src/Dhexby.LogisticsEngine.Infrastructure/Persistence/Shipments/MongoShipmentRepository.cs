using Dhexby.LogisticsEngine.Domain.Shipments;
using MongoDB.Driver;

namespace Dhexby.LogisticsEngine.Infrastructure.Persistence;

/// <summary>Repository di Shipment su MongoDB (collection "shipments").</summary>
internal sealed class MongoShipmentRepository : IShipmentRepository
{
    private readonly IMongoCollection<ShipmentDocument> _collection;

    public MongoShipmentRepository(MongoDbContext context)
    {
        _collection = context.GetCollection<ShipmentDocument>(MongoDbContext.ShipmentsCollection);
    }

    public async Task<Shipment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        ShipmentDocument? document = await _collection
            .Find(document => document.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        return document?.ToDomain();
    }

    public Task<bool> ExistsByTrackingNumberAsync(
        TrackingNumber trackingNumber,
        CancellationToken cancellationToken = default) =>
        _collection
            .Find(document => document.TrackingNumber == trackingNumber.Value)
            .AnyAsync(cancellationToken);

    public Task AddAsync(Shipment shipment, CancellationToken cancellationToken = default) =>
        _collection.InsertOneAsync(ShipmentDocument.FromDomain(shipment), options: null, cancellationToken);

    public async Task UpdateAsync(Shipment shipment, CancellationToken cancellationToken = default)
    {
        ReplaceOneResult result = await _collection.ReplaceOneAsync(
            document => document.Id == shipment.Id,
            ShipmentDocument.FromDomain(shipment),
            options: (ReplaceOptions?)null,
            cancellationToken);

        if (result.MatchedCount == 0)
        {
            throw new InvalidOperationException(
                $"Cannot update shipment '{shipment.Id}': not found in MongoDB.");
        }
    }
}
