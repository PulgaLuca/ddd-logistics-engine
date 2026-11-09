using LogisticsEngine.Application.Shipments;
using LogisticsEngine.SharedKernel;
using MediatR;

namespace LogisticsEngine.Application.Shipments.GetById;

public sealed record GetShipmentByIdQuery(Guid Id) : IRequest<Result<ShipmentDto>>;
