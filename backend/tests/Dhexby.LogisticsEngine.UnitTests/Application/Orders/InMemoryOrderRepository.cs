using Dhexby.LogisticsEngine.Domain.Orders;

namespace Dhexby.LogisticsEngine.UnitTests.Application.Orders;

/// <summary>Fake in-memory di <see cref="IOrderRepository"/> per i test degli handler.</summary>
internal sealed class InMemoryOrderRepository : IOrderRepository
{
    private readonly Dictionary<Guid, Order> _orders = [];

    public int UpdateCallCount { get; private set; }

    public Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(_orders.GetValueOrDefault(id));

    public Task<bool> ExistsByOrderNumberAsync(
        OrderNumber orderNumber,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(_orders.Values.Any(order => order.OrderNumber.Equals(orderNumber)));

    public Task<IReadOnlyList<Order>> GetPendingAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Order>>(
            _orders.Values.Where(order => order.Status == OrderStatus.Pending).ToArray());

    public Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        _orders.Add(order.Id, order);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
    {
        UpdateCallCount++;
        _orders[order.Id] = order;
        return Task.CompletedTask;
    }
}
