using Dhexby.LogisticsEngine.Domain.Orders;
using Dhexby.LogisticsEngine.Domain.Orders.Processing;
using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Console.Controllers;

/// <summary>Confronta l'ordine FIFO con la coda di dispatch per priorità.</summary>
public sealed class OrderQueueController
{
    private readonly IOrderProcessingQueue _processingQueue;
    private readonly IOrderDispatchPriorityQueue _priorityQueue;

    public OrderQueueController(
        IOrderProcessingQueue processingQueue,
        IOrderDispatchPriorityQueue priorityQueue)
    {
        _processingQueue = processingQueue;
        _priorityQueue = priorityQueue;
    }

    public void ShowDemo()
    {
        DateTime now = DateTime.UtcNow;
        Order[] orders =
        [
            CreateOrder("ORD-00000001", "Cliente A", OrderPriority.Standard, now.AddMinutes(-3)),
            CreateOrder("ORD-00000002", "Cliente B", OrderPriority.Priority, now.AddMinutes(-2)),
            CreateOrder("ORD-00000003", "Cliente C", OrderPriority.Express, now.AddMinutes(-1))
        ];

        System.Console.WriteLine("Coda di lavorazione FIFO:");
        Print(_processingQueue.Arrange(orders));
        System.Console.WriteLine("Coda di dispatch per priorità:");
        Print(_priorityQueue.Arrange(orders));
    }

    private static Order CreateOrder(string number, string customer, OrderPriority priority, DateTime placedOnUtc)
    {
        Result<OrderNumber> orderNumber = OrderNumber.Create(number);
        Result<CustomerName> customerName = CustomerName.Create(customer);
        return Order.Create(orderNumber.Value, customerName.Value, priority, placedOnUtc).Value;
    }

    private static void Print(IEnumerable<Order> orders)
    {
        foreach (Order order in orders)
        {
            System.Console.WriteLine($"{order.OrderNumber.Value} | {order.Priority} | {order.PlacedOnUtc:HH:mm:ss}");
        }
    }
}
