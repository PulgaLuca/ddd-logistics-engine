using LogisticsEngine.Domain.Shipments.Entities;
using LogisticsEngine.Domain.Shipments.Enums;

namespace LogisticsEngine.Application.Shipments;

public sealed record ShipmentDto(Guid Id, string TrackingNumber, ShipmentStatus Status)
{
    public static ShipmentDto From(Shipment shipment) => new(shipment.Id, shipment.TrackingNumber.Value, shipment.Status);
}
