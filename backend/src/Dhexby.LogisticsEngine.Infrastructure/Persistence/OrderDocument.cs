using Dhexby.LogisticsEngine.Domain.Orders;

namespace Dhexby.LogisticsEngine.Infrastructure.Persistence;

/// <summary>Modello di persistenza dell'ordine (solo primitivi).</summary>
internal sealed record OrderDocument(
    Guid Id,
    string OrderNumber,
    string CustomerName,
    OrderPriority Priority,
    OrderStatus Status,
    Guid? AssignedHubId,
    DateTime PlacedOnUtc)
{
    public static OrderDocument FromDomain(Order order) => new(
        order.Id,
        order.OrderNumber.Value,
        order.CustomerName.Value,
        order.Priority,
        order.Status,
        order.AssignedHubId,
        order.PlacedOnUtc);

    public Order ToDomain() => Order.Rehydrate(
        Id,
        Domain.Orders.OrderNumber.Create(OrderNumber).Value,
        Domain.Orders.CustomerName.Create(CustomerName).Value,
        Priority,
        Status,
        AssignedHubId,
        PlacedOnUtc);
}
