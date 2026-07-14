using Dhexby.LogisticsEngine.Domain.Shipments;

namespace Dhexby.LogisticsEngine.Infrastructure.Persistence;

/// <summary>
/// Modello di persistenza (solo primitivi, serializzabile in JSON).
/// Tiene il formato dello store fuori dal dominio.
/// </summary>
internal sealed record ShipmentDocument(
    Guid Id,
    string TrackingNumber,
    AddressDocument Origin,
    AddressDocument Destination,
    decimal WeightInKilograms,
    decimal PriceAmount,
    string PriceCurrency,
    bool IsExpress,
    ShipmentStatus Status,
    DateTime CreatedOnUtc,
    DateTime? DeliveredOnUtc)
{
    public static ShipmentDocument FromDomain(Shipment shipment) => new(
        shipment.Id,
        shipment.TrackingNumber.Value,
        AddressDocument.FromDomain(shipment.Origin),
        AddressDocument.FromDomain(shipment.Destination),
        shipment.Weight.Kilograms,
        shipment.Price.Amount,
        shipment.Price.Currency,
        shipment.IsExpress,
        shipment.Status,
        shipment.CreatedOnUtc,
        shipment.DeliveredOnUtc);

    public Shipment ToDomain() => Shipment.Rehydrate(
        Id,
        Domain.Shipments.TrackingNumber.Create(TrackingNumber).Value,
        Origin.ToDomain(),
        Destination.ToDomain(),
        Weight.Create(WeightInKilograms).Value,
        Money.Create(PriceAmount, PriceCurrency).Value,
        IsExpress,
        Status,
        CreatedOnUtc,
        DeliveredOnUtc);
}

internal sealed record AddressDocument(string Street, string City, string PostalCode, string Country)
{
    public static AddressDocument FromDomain(Address address) =>
        new(address.Street, address.City, address.PostalCode, address.Country);

    public Address ToDomain() => Address.Create(Street, City, PostalCode, Country).Value;
}
