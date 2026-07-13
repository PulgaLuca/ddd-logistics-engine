namespace Dhexby.LogisticsEngine.Domain.Orders.Processing;

/// <summary>
/// Coda di dispatch a priorità per l'express delivery: gli ordini a priorità
/// più alta vengono spediti per primi.
/// </summary>
public interface IOrderDispatchPriorityQueue
{
    IReadOnlyList<Order> Arrange(IEnumerable<Order> pendingOrders);
}
