using Dhexby.LogisticsEngine.Domain.Orders.Dispatching;

namespace Dhexby.LogisticsEngine.Console.Controllers;

/// <summary>Mostra la cronologia undo/redo gestita dalle due pile del dominio.</summary>
public sealed class DispatchHistoryController
{
    private readonly IDispatchHistory _history;

    public DispatchHistoryController(IDispatchHistory history)
    {
        _history = history;
    }

    public void ShowDemo()
    {
        Guid hubId = Guid.NewGuid();
        DispatchAction first = new(Guid.NewGuid(), hubId, DateTime.UtcNow.AddMinutes(-1));
        DispatchAction second = new(Guid.NewGuid(), hubId, DateTime.UtcNow);

        _history.RecordAssignment(first);
        _history.RecordAssignment(second);

        System.Console.WriteLine("Assegnazioni aggiunte: 2");
        PrintState();

        DispatchAction? undone = _history.CommitUndo();
        System.Console.WriteLine(undone is null
            ? "Nessuna assegnazione da annullare."
            : $"Undo: ordine {undone.OrderId} rimosso dalla cronologia attiva.");
        PrintState();

        DispatchAction? redone = _history.CommitRedo();
        System.Console.WriteLine(redone is null
            ? "Nessuna assegnazione da ripristinare."
            : $"Redo: ordine {redone.OrderId} ripristinato.");
        PrintState();
    }

    private void PrintState() =>
        System.Console.WriteLine($"Undo: {_history.UndoActions.Count}, Redo: {_history.RedoActions.Count}");
}
