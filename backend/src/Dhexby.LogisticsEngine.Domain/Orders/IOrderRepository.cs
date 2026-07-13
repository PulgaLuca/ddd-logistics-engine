namespace Dhexby.LogisticsEngine.Domain.Orders;

/// <summary>
/// Porta di persistenza dell'aggregato Order. <see cref="GetPendingAsync"/> è
/// pensato per alimentare in futuro la coda di processamento e la priority queue.
/// </summary>
public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> ExistsByOrderNumberAsync(OrderNumber orderNumber, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Order>> GetPendingAsync(CancellationToken cancellationToken = default);

    Task AddAsync(Order order, CancellationToken cancellationToken = default);

    Task UpdateAsync(Order order, CancellationToken cancellationToken = default);
}
