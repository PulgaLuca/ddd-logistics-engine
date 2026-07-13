using Dhexby.LogisticsEngine.Application.Orders.AssignOrder;
using Dhexby.LogisticsEngine.Domain.Hubs;
using Dhexby.LogisticsEngine.Domain.Orders;
using Dhexby.LogisticsEngine.Domain.Orders.Dispatching;
using Dhexby.LogisticsEngine.UnitTests.Application.Hubs;
using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.UnitTests.Application.Orders;

public class AssignOrderCommandHandlerTests
{
    private readonly InMemoryOrderRepository _orderRepository = new();
    private readonly InMemoryHubRepository _hubRepository = new();
    private readonly DispatchHistoryStack _dispatchHistory = new();
    private readonly AssignOrderCommandHandler _handler;

    public AssignOrderCommandHandlerTests()
    {
        _handler = new AssignOrderCommandHandler(_orderRepository, _hubRepository, _dispatchHistory);
    }

    private static Order CreateOrder() => Order.Create(
        OrderNumber.Create("ORD-ABC12345").Value,
        CustomerName.Create("Mario Rossi").Value,
        OrderPriority.Standard,
        DateTime.UtcNow).Value;

    private static Hub CreateHub() => Hub.Create(
        HubCode.Create("HUB-0001").Value,
        "Milano",
        GeoCoordinate.Create(45.46, 9.19).Value,
        DateTime.UtcNow).Value;

    [Fact]
    public async Task Handle_ShouldAssignOrder_WhenOrderAndHubExist()
    {
        Order order = CreateOrder();
        Hub hub = CreateHub();
        await _orderRepository.AddAsync(order);
        await _hubRepository.AddAsync(hub);

        Result result = await _handler.Handle(new AssignOrderCommand(order.Id, hub.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(OrderStatus.Assigned, order.Status);
        Assert.Equal(hub.Id, order.AssignedHubId);
        Assert.Equal(1, _orderRepository.UpdateCallCount);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenOrderDoesNotExist()
    {
        Hub hub = CreateHub();
        await _hubRepository.AddAsync(hub);
        Guid missingOrderId = Guid.NewGuid();

        Result result = await _handler.Handle(new AssignOrderCommand(missingOrderId, hub.Id), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(OrderErrors.NotFound(missingOrderId), result.Error);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenHubDoesNotExist()
    {
        Order order = CreateOrder();
        await _orderRepository.AddAsync(order);
        Guid missingHubId = Guid.NewGuid();

        Result result = await _handler.Handle(new AssignOrderCommand(order.Id, missingHubId), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(HubErrors.NotFound(missingHubId), result.Error);
        Assert.Equal(0, _orderRepository.UpdateCallCount);
    }
}
