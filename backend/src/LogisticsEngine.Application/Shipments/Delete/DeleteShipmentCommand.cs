using LogisticsEngine.SharedKernel;
using MediatR;

namespace LogisticsEngine.Application.Shipments.Delete;

public sealed record DeleteShipmentCommand(Guid Id) : IRequest<Result>;
