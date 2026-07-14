using Dhexby.LogisticsEngine.Domain.Hubs;
using MongoDB.Driver;

namespace Dhexby.LogisticsEngine.Infrastructure.Persistence;

/// <summary>Repository di Hub su MongoDB (collection "hubs").</summary>
internal sealed class MongoHubRepository : IHubRepository
{
    private readonly IMongoCollection<HubDocument> _collection;

    public MongoHubRepository(MongoDbContext context)
    {
        _collection = context.GetCollection<HubDocument>(MongoDbContext.HubsCollection);
    }

    public async Task<Hub?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        HubDocument? document = await _collection
            .Find(document => document.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        return document?.ToDomain();
    }

    public Task<bool> ExistsByCodeAsync(HubCode code, CancellationToken cancellationToken = default) =>
        _collection.Find(document => document.Code == code.Value).AnyAsync(cancellationToken);

    public async Task<IReadOnlyList<Hub>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        List<HubDocument> documents = await _collection
            .Find(FilterDefinition<HubDocument>.Empty)
            .ToListAsync(cancellationToken);

        return documents.Select(document => document.ToDomain()).ToArray();
    }

    public Task AddAsync(Hub hub, CancellationToken cancellationToken = default) =>
        _collection.InsertOneAsync(HubDocument.FromDomain(hub), options: null, cancellationToken);

    public async Task UpdateAsync(Hub hub, CancellationToken cancellationToken = default)
    {
        ReplaceOneResult result = await _collection.ReplaceOneAsync(
            document => document.Id == hub.Id,
            HubDocument.FromDomain(hub),
            options: (ReplaceOptions?)null,
            cancellationToken);

        if (result.MatchedCount == 0)
        {
            throw new InvalidOperationException($"Cannot update hub '{hub.Id}': not found in MongoDB.");
        }
    }
}
