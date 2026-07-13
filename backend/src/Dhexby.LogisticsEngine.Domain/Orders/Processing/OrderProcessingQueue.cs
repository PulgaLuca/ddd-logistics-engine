namespace Dhexby.LogisticsEngine.Domain.Orders.Processing;

/// <summary>
/// Struttura dati "Queue" (FIFO): gli ordini si lavorano nell'ordine di arrivo,
/// determinato dall'istante di inserimento (<see cref="Order.PlacedOnUtc"/>).
/// </summary>
public sealed class OrderProcessingQueue : IOrderProcessingQueue
{
    public IReadOnlyList<Order> Arrange(IEnumerable<Order> pendingOrders)
    {
        ArgumentNullException.ThrowIfNull(pendingOrders);

        var queue = new Queue<Order>(pendingOrders.OrderBy(order => order.PlacedOnUtc));

        var arranged = new List<Order>(queue.Count);
        while (queue.Count > 0)
        {
            arranged.Add(queue.Dequeue());
        }

        return arranged;
    }
}
