namespace Dhexby.LogisticsEngine.Domain.Orders.Dispatching;

/// <summary>
/// Struttura dati "Stack" per l'undo/redo del dispatch: due pile: quella di
/// undo contiene le assegnazioni applicate, quella di redo quelle annullate.
/// Thread-safe (accesso serializzato da un lock), essendo usata come singleton.
/// </summary>
public sealed class DispatchHistoryStack : IDispatchHistory
{
    private readonly Stack<DispatchAction> _undo = new();
    private readonly Stack<DispatchAction> _redo = new();
    private readonly object _lock = new();

    public bool CanUndo
    {
        get
        {
            lock (_lock)
            {
                return _undo.Count > 0;
            }
        }
    }

    public bool CanRedo
    {
        get
        {
            lock (_lock)
            {
                return _redo.Count > 0;
            }
        }
    }

    public IReadOnlyList<DispatchAction> UndoActions
    {
        get
        {
            lock (_lock)
            {
                return _undo.ToArray();
            }
        }
    }

    public IReadOnlyList<DispatchAction> RedoActions
    {
        get
        {
            lock (_lock)
            {
                return _redo.ToArray();
            }
        }
    }

    public void RecordAssignment(DispatchAction action)
    {
        ArgumentNullException.ThrowIfNull(action);

        lock (_lock)
        {
            _undo.Push(action);
            _redo.Clear();
        }
    }

    public DispatchAction? PeekUndo()
    {
        lock (_lock)
        {
            return _undo.Count > 0 ? _undo.Peek() : null;
        }
    }

    public DispatchAction? PeekRedo()
    {
        lock (_lock)
        {
            return _redo.Count > 0 ? _redo.Peek() : null;
        }
    }

    public DispatchAction? CommitUndo()
    {
        lock (_lock)
        {
            if (_undo.Count == 0)
            {
                return null;
            }

            DispatchAction action = _undo.Pop();
            _redo.Push(action);
            return action;
        }
    }

    public DispatchAction? CommitRedo()
    {
        lock (_lock)
        {
            if (_redo.Count == 0)
            {
                return null;
            }

            DispatchAction action = _redo.Pop();
            _undo.Push(action);
            return action;
        }
    }
}
