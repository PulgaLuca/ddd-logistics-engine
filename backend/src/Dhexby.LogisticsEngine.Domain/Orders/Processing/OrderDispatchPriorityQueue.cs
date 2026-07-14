namespace Dhexby.LogisticsEngine.Domain.Orders.Processing;

/// <summary>
/// Struttura dati "Priority Queue" per l'express delivery: gli ordini escono
/// dalla coda per priorità decrescente (Priority &gt; Express &gt; Standard) e, a
/// parità di priorità, per ordine di arrivo (FIFO).
/// </summary>
public sealed class OrderDispatchPriorityQueue : IOrderDispatchPriorityQueue
{
    public IReadOnlyList<Order> Arrange(IEnumerable<Order> pendingOrders)
    {
        ArgumentNullException.ThrowIfNull(pendingOrders);

        // Priorità composita: priorità più alta prima (valore negato, così il
        // minimo della coda è l'ordine più urgente); a parità, il più vecchio prima.
        var priorityQueue = new PriorityQueue<Order, (int PriorityRank, long PlacedTicks)>();

        foreach (Order order in pendingOrders)
        {
            priorityQueue.Enqueue(order, (-(int)order.Priority, order.PlacedOnUtc.Ticks));
        }

        var arranged = new List<Order>(priorityQueue.Count);
        while (priorityQueue.Count > 0)
        {
            arranged.Add(priorityQueue.Dequeue());
        }

        return arranged;
    }
}
