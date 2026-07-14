using Dhexby.LogisticsEngine.Domain.Shipments;
using Microsoft.Extensions.Options;

namespace Dhexby.LogisticsEngine.Infrastructure.Persistence;

/// <summary>
/// Repository di Shipment su file JSON, basato su <see cref="JsonEntityRepository{TDocument}"/>.
/// </summary>
internal sealed class JsonShipmentRepository : JsonEntityRepository<ShipmentDocument>, IShipmentRepository
{
    public JsonShipmentRepository(IOptions<JsonStorageOptions> options)
        : base(options.Value.ShipmentsFilePath)
    {
    }

    public Task<Shipment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        QueryAsync(documents => documents.Find(document => document.Id == id)?.ToDomain(), cancellationToken);

    public Task<bool> ExistsByTrackingNumberAsync(
        TrackingNumber trackingNumber,
        CancellationToken cancellationToken = default) =>
        QueryAsync(
            documents => documents.Exists(document => document.TrackingNumber == trackingNumber.Value),
            cancellationToken);

    public Task AddAsync(Shipment shipment, CancellationToken cancellationToken = default) =>
        MutateAsync(documents => documents.Add(ShipmentDocument.FromDomain(shipment)), cancellationToken);

    public Task UpdateAsync(Shipment shipment, CancellationToken cancellationToken = default) =>
        MutateAsync(
            documents =>
            {
                int index = documents.FindIndex(document => document.Id == shipment.Id);
                if (index < 0)
                {
                    throw new InvalidOperationException(
                        $"Cannot update shipment '{shipment.Id}': not found in the JSON store.");
                }

                documents[index] = ShipmentDocument.FromDomain(shipment);
            },
            cancellationToken);
}
