using Dhexby.LogisticsEngine.Domain.Shipments;

namespace Dhexby.LogisticsEngine.Application.Shipments;

/// <summary>
/// DTO di lettura: espone solo primitivi, così i layer esterni
/// non dipendono dagli oggetti di dominio.
/// </summary>
public sealed record ShipmentResponse(
    Guid Id,
    string TrackingNumber,
    string OriginCity,
    string DestinationCity,
    decimal WeightInKilograms,
    decimal PriceAmount,
    string PriceCurrency,
    bool IsExpress,
    string Status,
    DateTime CreatedOnUtc,
    DateTime? DeliveredOnUtc)
{
    public static ShipmentResponse FromDomain(Shipment shipment) => new(
        shipment.Id,
        shipment.TrackingNumber.Value,
        shipment.Origin.City,
        shipment.Destination.City,
        shipment.Weight.Kilograms,
        shipment.Price.Amount,
        shipment.Price.Currency,
        shipment.IsExpress,
        shipment.Status.ToString(),
        shipment.CreatedOnUtc,
        shipment.DeliveredOnUtc);
}
