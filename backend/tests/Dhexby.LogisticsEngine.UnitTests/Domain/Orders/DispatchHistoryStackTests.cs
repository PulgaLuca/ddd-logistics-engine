using Dhexby.LogisticsEngine.Domain.Orders.Dispatching;

namespace Dhexby.LogisticsEngine.UnitTests.Domain.Orders;

public class DispatchHistoryStackTests
{
    private readonly DispatchHistoryStack _history = new();

    private static DispatchAction NewAction() => new(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow);

    [Fact]
    public void RecordAssignment_ShouldEnableUndo_AndClearRedo()
    {
        _history.RecordAssignment(NewAction());

        Assert.True(_history.CanUndo);
        Assert.False(_history.CanRedo);
        Assert.Single(_history.UndoActions);
    }

    [Fact]
    public void CommitUndo_ShouldMoveActionToRedo()
    {
        DispatchAction action = NewAction();
        _history.RecordAssignment(action);

        DispatchAction? undone = _history.CommitUndo();

        Assert.Equal(action, undone);
        Assert.False(_history.CanUndo);
        Assert.True(_history.CanRedo);
    }

    [Fact]
    public void CommitRedo_ShouldMoveActionBackToUndo()
    {
        DispatchAction action = NewAction();
        _history.RecordAssignment(action);
        _history.CommitUndo();

        DispatchAction? redone = _history.CommitRedo();

        Assert.Equal(action, redone);
        Assert.True(_history.CanUndo);
        Assert.False(_history.CanRedo);
    }

    [Fact]
    public void RecordAssignment_ShouldDiscardRedoHistory()
    {
        _history.RecordAssignment(NewAction());
        _history.CommitUndo();
        Assert.True(_history.CanRedo);

        _history.RecordAssignment(NewAction());

        Assert.False(_history.CanRedo);
    }

    [Fact]
    public void Peek_ShouldReturnTop_WithoutRemoving()
    {
        DispatchAction action = NewAction();
        _history.RecordAssignment(action);

        Assert.Equal(action, _history.PeekUndo());
        Assert.True(_history.CanUndo);
    }

    [Fact]
    public void Commit_ShouldReturnNull_WhenEmpty()
    {
        Assert.Null(_history.CommitUndo());
        Assert.Null(_history.CommitRedo());
    }
}
