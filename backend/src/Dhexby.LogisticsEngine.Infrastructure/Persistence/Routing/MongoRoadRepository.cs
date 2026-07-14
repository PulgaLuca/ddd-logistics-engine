using Dhexby.LogisticsEngine.Domain.Routing;
using MongoDB.Driver;

namespace Dhexby.LogisticsEngine.Infrastructure.Persistence;

/// <summary>Repository di Road su MongoDB (collection "roads").</summary>
internal sealed class MongoRoadRepository : IRoadRepository
{
    private readonly IMongoCollection<RoadDocument> _collection;

    public MongoRoadRepository(MongoDbContext context)
    {
        _collection = context.GetCollection<RoadDocument>(MongoDbContext.RoadsCollection);
    }

    public async Task<Road?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        RoadDocument? document = await _collection
            .Find(document => document.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        return document?.ToDomain();
    }

    public Task<bool> ExistsBetweenAsync(
        Guid firstHubId,
        Guid secondHubId,
        CancellationToken cancellationToken = default) =>
        _collection
            .Find(document =>
                (document.FromHubId == firstHubId && document.ToHubId == secondHubId) ||
                (document.FromHubId == secondHubId && document.ToHubId == firstHubId))
            .AnyAsync(cancellationToken);

    public async Task<IReadOnlyList<Road>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        List<RoadDocument> documents = await _collection
            .Find(FilterDefinition<RoadDocument>.Empty)
            .ToListAsync(cancellationToken);

        return documents.Select(document => document.ToDomain()).ToArray();
    }

    public Task AddAsync(Road road, CancellationToken cancellationToken = default) =>
        _collection.InsertOneAsync(RoadDocument.FromDomain(road), options: null, cancellationToken);
}
