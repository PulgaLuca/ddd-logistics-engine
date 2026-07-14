using Dhexby.LogisticsEngine.Domain.Orders;

namespace Dhexby.LogisticsEngine.Application.Orders;

/// <summary>DTO di lettura dell'ordine: solo primitivi.</summary>
public sealed record OrderResponse(
    Guid Id,
    string OrderNumber,
    string CustomerName,
    string Priority,
    string Status,
    Guid? AssignedHubId,
    DateTime PlacedOnUtc)
{
    public static OrderResponse FromDomain(Order order) => new(
        order.Id,
        order.OrderNumber.Value,
        order.CustomerName.Value,
        order.Priority.ToString(),
        order.Status.ToString(),
        order.AssignedHubId,
        order.PlacedOnUtc);
}
