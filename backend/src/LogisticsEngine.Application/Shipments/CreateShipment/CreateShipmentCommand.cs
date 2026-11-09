using LogisticsEngine.SharedKernel;
using MediatR;

namespace LogisticsEngine.Application.Shipments.CreateShipment;

public sealed record CreateShipmentCommand(string? TrackingNumber) : IRequest<Result<ShipmentDto>>;
