using Dhexby.LogisticsEngine.Domain.Orders;
using Microsoft.Extensions.Options;

namespace Dhexby.LogisticsEngine.Infrastructure.Persistence;

/// <summary>Repository di Order su file JSON, basato sulla base condivisa.</summary>
internal sealed class JsonOrderRepository : JsonEntityRepository<OrderDocument>, IOrderRepository
{
    public JsonOrderRepository(IOptions<JsonStorageOptions> options)
        : base(options.Value.OrdersFilePath)
    {
    }

    public Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        QueryAsync(documents => documents.Find(document => document.Id == id)?.ToDomain(), cancellationToken);

    public Task<bool> ExistsByOrderNumberAsync(
        OrderNumber orderNumber,
        CancellationToken cancellationToken = default) =>
        QueryAsync(
            documents => documents.Exists(document => document.OrderNumber == orderNumber.Value),
            cancellationToken);

    public Task<IReadOnlyList<Order>> GetPendingAsync(CancellationToken cancellationToken = default) =>
        QueryAsync<IReadOnlyList<Order>>(
            documents => documents
                .Where(document => document.Status == OrderStatus.Pending)
                .Select(document => document.ToDomain())
                .ToArray(),
            cancellationToken);

    public Task AddAsync(Order order, CancellationToken cancellationToken = default) =>
        MutateAsync(documents => documents.Add(OrderDocument.FromDomain(order)), cancellationToken);

    public Task UpdateAsync(Order order, CancellationToken cancellationToken = default) =>
        MutateAsync(
            documents =>
            {
                int index = documents.FindIndex(document => document.Id == order.Id);
                if (index < 0)
                {
                    throw new InvalidOperationException(
                        $"Cannot update order '{order.Id}': not found in the JSON store.");
                }

                documents[index] = OrderDocument.FromDomain(order);
            },
            cancellationToken);
}
