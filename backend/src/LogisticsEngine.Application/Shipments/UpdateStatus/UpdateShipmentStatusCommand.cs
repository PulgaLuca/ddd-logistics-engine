using LogisticsEngine.Domain.Shipments.Enums;
using LogisticsEngine.SharedKernel;
using MediatR;

namespace LogisticsEngine.Application.Shipments.UpdateStatus;

public sealed record UpdateShipmentStatusCommand(Guid Id, ShipmentStatus Status) : IRequest<Result>;
