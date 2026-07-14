using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Orders.GetOrderById;

public sealed record GetOrderByIdQuery(Guid OrderId) : IRequest<Result<OrderResponse>>;
