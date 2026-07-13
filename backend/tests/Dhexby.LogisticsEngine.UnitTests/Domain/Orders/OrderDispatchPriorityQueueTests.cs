using Dhexby.LogisticsEngine.Domain.Orders;
using Dhexby.LogisticsEngine.Domain.Orders.Processing;

namespace Dhexby.LogisticsEngine.UnitTests.Domain.Orders;

public class OrderDispatchPriorityQueueTests
{
    private readonly OrderDispatchPriorityQueue _queue = new();

    [Fact]
    public void Arrange_ShouldDispatchHigherPriorityFirst()
    {
        var baseTime = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        Order standard = OrderQueueTestData.Pending(OrderPriority.Standard, baseTime);
        Order express = OrderQueueTestData.Pending(OrderPriority.Express, baseTime.AddMinutes(5));
        Order priority = OrderQueueTestData.Pending(OrderPriority.Priority, baseTime.AddMinutes(10));

        IReadOnlyList<Order> arranged = _queue.Arrange([standard, express, priority]);

        Assert.Equal([priority.Id, express.Id, standard.Id], arranged.Select(order => order.Id));
    }

    [Fact]
    public void Arrange_ShouldBreakTiesByArrivalTime_Fifo()
    {
        var baseTime = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        Order older = OrderQueueTestData.Pending(OrderPriority.Express, baseTime);
        Order newer = OrderQueueTestData.Pending(OrderPriority.Express, baseTime.AddMinutes(5));

        IReadOnlyList<Order> arranged = _queue.Arrange([newer, older]);

        Assert.Equal([older.Id, newer.Id], arranged.Select(order => order.Id));
    }
}
