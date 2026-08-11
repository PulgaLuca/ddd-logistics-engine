using Dhexby.LogisticsEngine.Domain.Orders;
using Microsoft.EntityFrameworkCore;

namespace Dhexby.LogisticsEngine.Infrastructure.Persistence;

/// <summary>Repository di Order su PostgreSQL (tabella "orders").</summary>
internal sealed class PostgresOrderRepository : IOrderRepository
{
    private readonly LogisticsDbContext _context;

    public PostgresOrderRepository(LogisticsDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        OrderDocument? document = await _context.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(document => document.Id == id, cancellationToken);

        return document?.ToDomain();
    }

    public Task<bool> ExistsByOrderNumberAsync(
        OrderNumber orderNumber,
        CancellationToken cancellationToken = default) =>
        _context.Orders
            .AsNoTracking()
            .AnyAsync(document => document.OrderNumber == orderNumber.Value, cancellationToken);

    public async Task<IReadOnlyList<Order>> GetPendingAsync(CancellationToken cancellationToken = default)
    {
        List<OrderDocument> documents = await _context.Orders
            .AsNoTracking()
            .Where(document => document.Status == OrderStatus.Pending)
            .ToListAsync(cancellationToken);

        return documents.Select(document => document.ToDomain()).ToArray();
    }

    public async Task AddAsync(Order order, CancellationToken cancellationToken = default) =>
        await _context.Orders.AddAsync(OrderDocument.FromDomain(order), cancellationToken);

    public async Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(order);

        bool exists = await _context.Orders
            .AsNoTracking()
            .AnyAsync(document => document.Id == order.Id, cancellationToken);

        if (!exists)
        {
            throw new InvalidOperationException($"Cannot update order '{order.Id}': not found in PostgreSQL.");
        }

        _context.Orders.Update(OrderDocument.FromDomain(order));
    }
}
