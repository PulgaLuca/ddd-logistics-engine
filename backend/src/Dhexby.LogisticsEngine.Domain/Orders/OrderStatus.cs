namespace Dhexby.LogisticsEngine.Domain.Orders;

/// <summary>
/// Ciclo di vita dell'ordine.
/// Pending -&gt; Assigned -&gt; Completed; Cancelled da Pending/Assigned.
/// L'accoppiata Assigned/Pending (Assign/Unassign) abilita undo/redo futuri.
/// </summary>
public enum OrderStatus
{
    Pending = 0,
    Assigned = 1,
    Completed = 2,
    Cancelled = 3
}
