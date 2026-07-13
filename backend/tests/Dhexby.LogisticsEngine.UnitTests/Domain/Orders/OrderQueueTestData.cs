using Dhexby.LogisticsEngine.Domain.Orders;

namespace Dhexby.LogisticsEngine.UnitTests.Domain.Orders;

/// <summary>Helper condiviso per costruire ordini con priorità e istante di arrivo controllati.</summary>
internal static class OrderQueueTestData
{
    private static int _counter;

    public static Order Pending(OrderPriority priority, DateTime placedOnUtc)
    {
        int index = Interlocked.Increment(ref _counter);
        string suffix = index.ToString("D8", System.Globalization.CultureInfo.InvariantCulture)[^8..];

        return Order.Rehydrate(
            Guid.NewGuid(),
            OrderNumber.Create($"ORD-{suffix}").Value,
            CustomerName.Create($"Customer {index}").Value,
            priority,
            OrderStatus.Pending,
            assignedHubId: null,
            placedOnUtc);
    }
}
