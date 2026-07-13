using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Orders.AssignOrder;

public sealed record AssignOrderCommand(Guid OrderId, Guid HubId) : IRequest<Result>;
