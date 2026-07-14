using Dhexby.LogisticsEngine.Domain.Orders;
using MongoDB.Driver;

namespace Dhexby.LogisticsEngine.Infrastructure.Persistence;

/// <summary>Repository di Order su MongoDB (collection "orders").</summary>
internal sealed class MongoOrderRepository : IOrderRepository
{
    private readonly IMongoCollection<OrderDocument> _collection;

    public MongoOrderRepository(MongoDbContext context)
    {
        _collection = context.GetCollection<OrderDocument>(MongoDbContext.OrdersCollection);
    }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        OrderDocument? document = await _collection
            .Find(document => document.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        return document?.ToDomain();
    }

    public Task<bool> ExistsByOrderNumberAsync(
        OrderNumber orderNumber,
        CancellationToken cancellationToken = default) =>
        _collection
            .Find(document => document.OrderNumber == orderNumber.Value)
            .AnyAsync(cancellationToken);

    public async Task<IReadOnlyList<Order>> GetPendingAsync(CancellationToken cancellationToken = default)
    {
        List<OrderDocument> documents = await _collection
            .Find(document => document.Status == OrderStatus.Pending)
            .ToListAsync(cancellationToken);

        return documents.Select(document => document.ToDomain()).ToArray();
    }

    public Task AddAsync(Order order, CancellationToken cancellationToken = default) =>
        _collection.InsertOneAsync(OrderDocument.FromDomain(order), options: null, cancellationToken);

    public async Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
    {
        ReplaceOneResult result = await _collection.ReplaceOneAsync(
            document => document.Id == order.Id,
            OrderDocument.FromDomain(order),
            options: (ReplaceOptions?)null,
            cancellationToken);

        if (result.MatchedCount == 0)
        {
            throw new InvalidOperationException($"Cannot update order '{order.Id}': not found in MongoDB.");
        }
    }
}
