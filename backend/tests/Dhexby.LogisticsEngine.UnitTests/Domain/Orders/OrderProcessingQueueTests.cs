using Dhexby.LogisticsEngine.Domain.Orders;
using Dhexby.LogisticsEngine.Domain.Orders.Processing;

namespace Dhexby.LogisticsEngine.UnitTests.Domain.Orders;

public class OrderProcessingQueueTests
{
    private readonly OrderProcessingQueue _queue = new();

    [Fact]
    public void Arrange_ShouldOrderByArrivalTime_Fifo()
    {
        var baseTime = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        Order third = OrderQueueTestData.Pending(OrderPriority.Priority, baseTime.AddMinutes(30));
        Order first = OrderQueueTestData.Pending(OrderPriority.Standard, baseTime);
        Order second = OrderQueueTestData.Pending(OrderPriority.Express, baseTime.AddMinutes(10));

        IReadOnlyList<Order> arranged = _queue.Arrange([third, first, second]);

        // FIFO puro: conta solo l'ordine di arrivo, non la priorità.
        Assert.Equal([first.Id, second.Id, third.Id], arranged.Select(order => order.Id));
    }

    [Fact]
    public void Arrange_ShouldReturnEmpty_WhenNoOrders()
    {
        Assert.Empty(_queue.Arrange([]));
    }
}
