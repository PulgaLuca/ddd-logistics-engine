using LogisticsEngine.Application.Shipments;
using MediatR;

namespace LogisticsEngine.Application.Shipments.GetAll;

public sealed record GetAllShipmentsQuery : IRequest<IReadOnlyList<ShipmentDto>>;
