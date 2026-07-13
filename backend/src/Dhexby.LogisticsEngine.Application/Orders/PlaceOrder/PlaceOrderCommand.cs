using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Orders.PlaceOrder;

public sealed record PlaceOrderCommand(
    string OrderNumber,
    string CustomerName,
    string Priority) : IRequest<Result<Guid>>;
