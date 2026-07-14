namespace Dhexby.LogisticsEngine.Domain.Orders.Dispatching;

/// <summary>
/// Cronologia delle assegnazioni con semantica undo/redo (due Stack).
/// L'implementazione è un singleton in-memory: mantiene lo stato tra le
/// richieste per il tempo di vita del processo.
/// </summary>
public interface IDispatchHistory
{
    bool CanUndo { get; }

    bool CanRedo { get; }

    /// <summary>Azioni annullabili, dalla più recente alla più vecchia.</summary>
    IReadOnlyList<DispatchAction> UndoActions { get; }

    /// <summary>Azioni ripetibili, dalla più recente alla più vecchia.</summary>
    IReadOnlyList<DispatchAction> RedoActions { get; }

    /// <summary>Registra una nuova assegnazione (svuota la pila di redo).</summary>
    void RecordAssignment(DispatchAction action);

    /// <summary>Restituisce l'azione in cima alla pila di undo senza rimuoverla.</summary>
    DispatchAction? PeekUndo();

    /// <summary>Restituisce l'azione in cima alla pila di redo senza rimuoverla.</summary>
    DispatchAction? PeekRedo();

    /// <summary>Sposta l'azione in cima da undo a redo e la restituisce.</summary>
    DispatchAction? CommitUndo();

    /// <summary>Sposta l'azione in cima da redo a undo e la restituisce.</summary>
    DispatchAction? CommitRedo();
}
