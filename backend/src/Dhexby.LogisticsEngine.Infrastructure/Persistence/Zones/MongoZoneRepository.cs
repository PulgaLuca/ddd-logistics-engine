using Dhexby.LogisticsEngine.Domain.Zones;
using MongoDB.Driver;

namespace Dhexby.LogisticsEngine.Infrastructure.Persistence;

/// <summary>Repository di Zone su MongoDB (collection "zones").</summary>
internal sealed class MongoZoneRepository : IZoneRepository
{
    private readonly IMongoCollection<ZoneDocument> _collection;

    public MongoZoneRepository(MongoDbContext context)
    {
        _collection = context.GetCollection<ZoneDocument>(MongoDbContext.ZonesCollection);
    }

    public async Task<Zone?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        ZoneDocument? document = await _collection
            .Find(document => document.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        return document?.ToDomain();
    }

    public Task<bool> ExistsByCodeAsync(ZoneCode code, CancellationToken cancellationToken = default) =>
        _collection.Find(document => document.Code == code.Value).AnyAsync(cancellationToken);

    public async Task<IReadOnlyList<Zone>> GetChildrenAsync(
        Guid parentZoneId,
        CancellationToken cancellationToken = default)
    {
        List<ZoneDocument> documents = await _collection
            .Find(document => document.ParentZoneId == parentZoneId)
            .ToListAsync(cancellationToken);

        return documents.Select(document => document.ToDomain()).ToArray();
    }

    public Task AddAsync(Zone zone, CancellationToken cancellationToken = default) =>
        _collection.InsertOneAsync(ZoneDocument.FromDomain(zone), options: null, cancellationToken);

    public async Task UpdateAsync(Zone zone, CancellationToken cancellationToken = default)
    {
        ReplaceOneResult result = await _collection.ReplaceOneAsync(
            document => document.Id == zone.Id,
            ZoneDocument.FromDomain(zone),
            options: (ReplaceOptions?)null,
            cancellationToken);

        if (result.MatchedCount == 0)
        {
            throw new InvalidOperationException($"Cannot update zone '{zone.Id}': not found in MongoDB.");
        }
    }
}
