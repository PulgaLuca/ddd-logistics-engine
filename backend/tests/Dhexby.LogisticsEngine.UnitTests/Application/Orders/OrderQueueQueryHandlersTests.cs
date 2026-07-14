using Dhexby.LogisticsEngine.Application.Orders.Processing;
using Dhexby.LogisticsEngine.Application.Orders.Processing.GetExpressDispatchQueue;
using Dhexby.LogisticsEngine.Application.Orders.Processing.GetOrderProcessingQueue;
using Dhexby.LogisticsEngine.Domain.Orders;
using Dhexby.LogisticsEngine.Domain.Orders.Processing;
using Dhexby.LogisticsEngine.UnitTests.Domain.Orders;
using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.UnitTests.Application.Orders;

public class OrderQueueQueryHandlersTests
{
    private readonly InMemoryOrderRepository _repository = new();
    private static readonly DateTime _baseTime = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    private async Task<(Order Standard, Order Express, Order Priority)> SeedPendingAsync()
    {
        Order standard = OrderQueueTestData.Pending(OrderPriority.Standard, _baseTime);
        Order express = OrderQueueTestData.Pending(OrderPriority.Express, _baseTime.AddMinutes(10));
        Order priority = OrderQueueTestData.Pending(OrderPriority.Priority, _baseTime.AddMinutes(20));

        await _repository.AddAsync(priority);
        await _repository.AddAsync(standard);
        await _repository.AddAsync(express);

        return (standard, express, priority);
    }

    [Fact]
    public async Task ProcessingQueue_ShouldReturnFifo_WithPositions()
    {
        (Order standard, Order express, Order priority) = await SeedPendingAsync();
        var handler = new GetOrderProcessingQueueQueryHandler(_repository, new OrderProcessingQueue());

        Result<OrderQueueResponse> result = await handler.Handle(
            new GetOrderProcessingQueueQuery(),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Fifo", result.Value.Strategy);
        Assert.Equal(3, result.Value.Count);
        Assert.Equal(
            [standard.Id, express.Id, priority.Id],
            result.Value.Items.Select(item => item.OrderId));
        Assert.Equal([1, 2, 3], result.Value.Items.Select(item => item.Position));
    }

    [Fact]
    public async Task ExpressQueue_ShouldReturnByPriority()
    {
        (Order standard, Order express, Order priority) = await SeedPendingAsync();
        var handler = new GetExpressDispatchQueueQueryHandler(_repository, new OrderDispatchPriorityQueue());

        Result<OrderQueueResponse> result = await handler.Handle(
            new GetExpressDispatchQueueQuery(),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Priority", result.Value.Strategy);
        Assert.Equal(
            [priority.Id, express.Id, standard.Id],
            result.Value.Items.Select(item => item.OrderId));
    }
}
