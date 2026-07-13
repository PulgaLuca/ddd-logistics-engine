using Dhexby.LogisticsEngine.Application.Orders.Dispatching;
using Dhexby.LogisticsEngine.Application.Orders.Dispatching.RedoDispatch;
using Dhexby.LogisticsEngine.Application.Orders.Dispatching.UndoDispatch;
using Dhexby.LogisticsEngine.Domain.Orders;
using Dhexby.LogisticsEngine.Domain.Orders.Dispatching;
using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.UnitTests.Application.Orders;

public class DispatchUndoRedoHandlerTests
{
    private readonly InMemoryOrderRepository _orderRepository = new();
    private readonly DispatchHistoryStack _history = new();
    private readonly UndoDispatchCommandHandler _undoHandler;
    private readonly RedoDispatchCommandHandler _redoHandler;

    public DispatchUndoRedoHandlerTests()
    {
        _undoHandler = new UndoDispatchCommandHandler(_orderRepository, _history);
        _redoHandler = new RedoDispatchCommandHandler(_orderRepository, _history);
    }

    private async Task<(Order Order, Guid HubId)> SeedAssignedOrderAsync()
    {
        Order order = Order.Create(
            OrderNumber.Create("ORD-ABC12345").Value,
            CustomerName.Create("Mario Rossi").Value,
            OrderPriority.Standard,
            DateTime.UtcNow).Value;

        Guid hubId = Guid.NewGuid();
        order.AssignToHub(hubId);
        await _orderRepository.AddAsync(order);
        _history.RecordAssignment(new DispatchAction(order.Id, hubId, DateTime.UtcNow));

        return (order, hubId);
    }

    [Fact]
    public async Task Undo_ShouldRevertAssignment_AndEnableRedo()
    {
        (Order order, _) = await SeedAssignedOrderAsync();

        Result<DispatchHistoryResponse> result = await _undoHandler.Handle(
            new UndoDispatchCommand(),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.Null(order.AssignedHubId);
        Assert.False(result.Value.CanUndo);
        Assert.True(result.Value.CanRedo);
    }

    [Fact]
    public async Task Redo_ShouldReapplyAssignment_ToSameHub()
    {
        (Order order, Guid hubId) = await SeedAssignedOrderAsync();
        await _undoHandler.Handle(new UndoDispatchCommand(), CancellationToken.None);

        Result<DispatchHistoryResponse> result = await _redoHandler.Handle(
            new RedoDispatchCommand(),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(OrderStatus.Assigned, order.Status);
        Assert.Equal(hubId, order.AssignedHubId);
        Assert.True(result.Value.CanUndo);
        Assert.False(result.Value.CanRedo);
    }

    [Fact]
    public async Task Undo_ShouldFail_WhenHistoryEmpty()
    {
        Result<DispatchHistoryResponse> result = await _undoHandler.Handle(
            new UndoDispatchCommand(),
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(DispatchErrors.NothingToUndo, result.Error);
    }

    [Fact]
    public async Task Redo_ShouldFail_WhenNothingUndone()
    {
        await SeedAssignedOrderAsync();

        Result<DispatchHistoryResponse> result = await _redoHandler.Handle(
            new RedoDispatchCommand(),
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(DispatchErrors.NothingToRedo, result.Error);
    }
}
