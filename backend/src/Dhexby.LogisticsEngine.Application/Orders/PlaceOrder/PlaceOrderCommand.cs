using Dhexby.LogisticsEngine.Application.Abstractions.Messaging;

namespace Dhexby.LogisticsEngine.Application.Orders.PlaceOrder;

public sealed record PlaceOrderCommand(
    string OrderNumber,
    string CustomerName,
    string Priority) : ICommand<Guid>;
