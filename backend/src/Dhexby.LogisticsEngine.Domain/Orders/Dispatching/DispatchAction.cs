namespace Dhexby.LogisticsEngine.Domain.Orders.Dispatching;

/// <summary>
/// Registrazione immutabile di un'assegnazione ordine -&gt; hub, elemento della
/// cronologia di dispatch usata per undo/redo.
/// </summary>
public sealed record DispatchAction(Guid OrderId, Guid HubId, DateTime OccurredOnUtc);
