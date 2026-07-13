namespace Dhexby.LogisticsEngine.Domain.Orders.Processing;

/// <summary>
/// Coda di processamento degli ordini (FIFO): dispone gli ordini in attesa
/// nell'ordine in cui vanno lavorati.
/// </summary>
public interface IOrderProcessingQueue
{
    IReadOnlyList<Order> Arrange(IEnumerable<Order> pendingOrders);
}
