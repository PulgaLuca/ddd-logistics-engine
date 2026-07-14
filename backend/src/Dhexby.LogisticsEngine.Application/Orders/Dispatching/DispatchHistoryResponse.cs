using Dhexby.LogisticsEngine.Domain.Orders.Dispatching;

namespace Dhexby.LogisticsEngine.Application.Orders.Dispatching;

/// <summary>
/// Stato della cronologia di dispatch: flag di disponibilità undo/redo e
/// contenuto delle due pile (dalla più recente alla più vecchia). Forma stabile
/// per il frontend.
/// </summary>
public sealed record DispatchHistoryResponse(
    bool CanUndo,
    bool CanRedo,
    IReadOnlyList<DispatchActionResponse> UndoStack,
    IReadOnlyList<DispatchActionResponse> RedoStack)
{
    public static DispatchHistoryResponse From(IDispatchHistory history) => new(
        history.CanUndo,
        history.CanRedo,
        history.UndoActions.Select(DispatchActionResponse.From).ToArray(),
        history.RedoActions.Select(DispatchActionResponse.From).ToArray());
}

public sealed record DispatchActionResponse(Guid OrderId, Guid HubId, DateTime OccurredOnUtc)
{
    public static DispatchActionResponse From(DispatchAction action) =>
        new(action.OrderId, action.HubId, action.OccurredOnUtc);
}
