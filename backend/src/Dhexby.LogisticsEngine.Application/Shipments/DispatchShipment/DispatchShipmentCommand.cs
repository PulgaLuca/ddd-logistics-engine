using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Shipments.DispatchShipment;

public sealed record DispatchShipmentCommand(Guid ShipmentId) : IRequest<Result>;
