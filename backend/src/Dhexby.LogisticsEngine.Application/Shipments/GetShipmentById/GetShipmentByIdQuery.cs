using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Shipments.GetShipmentById;

public sealed record GetShipmentByIdQuery(Guid ShipmentId) : IRequest<Result<ShipmentResponse>>;
