using LogisticsEngine.Domain.Shipments.Enums;

namespace LogisticsEngine.Application.Shipments.UpdateStatus;

public sealed record UpdateShipmentStatusCommand(Guid Id, ShipmentStatus Status);
