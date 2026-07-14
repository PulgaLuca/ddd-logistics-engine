using Dhexby.LogisticsEngine.Application.Shipments;
using Dhexby.LogisticsEngine.Application.Shipments.CreateShipment;

namespace Dhexby.LogisticsEngine.Api.Controllers;

/// <summary>Contratto HTTP di creazione: tradotto nel command applicativo.</summary>
public sealed record CreateShipmentRequest(
    string TrackingNumber,
    AddressRequest Origin,
    AddressRequest Destination,
    decimal WeightInKilograms,
    bool IsExpress)
{
    public CreateShipmentCommand ToCommand() => new(
        TrackingNumber,
        Origin.ToDto(),
        Destination.ToDto(),
        WeightInKilograms,
        IsExpress);
}

public sealed record AddressRequest(string Street, string City, string PostalCode, string Country)
{
    public AddressDto ToDto() => new(Street, City, PostalCode, Country);
}
