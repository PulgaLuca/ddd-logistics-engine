namespace Dhexby.LogisticsEngine.Domain.Orders;

/// <summary>
/// Priorità dell'ordine. Il valore numerico crescente indica priorità maggiore:
/// è già ordinabile per una futura Priority Queue (OrderDispatchPriorityQueue).
/// </summary>
public enum OrderPriority
{
    Standard = 0,
    Express = 1,
    Priority = 2
}
