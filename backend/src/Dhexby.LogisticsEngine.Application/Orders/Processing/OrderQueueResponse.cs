using Dhexby.LogisticsEngine.Domain.Orders;

namespace Dhexby.LogisticsEngine.Application.Orders.Processing;

/// <summary>
/// Risultato "sequenza ordinata" restituito dai servizi basati su code.
/// Il tipo è volutamente generico e stabile (strategia + elenco ordinato con
/// posizione): i futuri servizi di percorso (BFS/DFS/Dijkstra/A*) potranno
/// riusare la stessa forma, così il contratto verso il frontend non cambia.
/// </summary>
public sealed record OrderQueueResponse(
    string Strategy,
    int Count,
    IReadOnlyList<OrderQueueItemResponse> Items)
{
    public static OrderQueueResponse From(string strategy, IReadOnlyList<Order> orderedOrders)
    {
        var items = new List<OrderQueueItemResponse>(orderedOrders.Count);
        for (int position = 0; position < orderedOrders.Count; position++)
        {
            items.Add(OrderQueueItemResponse.From(position + 1, orderedOrders[position]));
        }

        return new OrderQueueResponse(strategy, items.Count, items);
    }
}

/// <summary>Un elemento della sequenza, con la sua posizione (1-based).</summary>
public sealed record OrderQueueItemResponse(
    int Position,
    Guid OrderId,
    string OrderNumber,
    string CustomerName,
    string Priority,
    DateTime PlacedOnUtc)
{
    public static OrderQueueItemResponse From(int position, Order order) => new(
        position,
        order.Id,
        order.OrderNumber.Value,
        order.CustomerName.Value,
        order.Priority.ToString(),
        order.PlacedOnUtc);
}
