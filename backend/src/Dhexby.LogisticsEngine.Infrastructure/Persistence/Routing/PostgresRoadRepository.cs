using Dhexby.LogisticsEngine.Domain.Routing;
using Microsoft.EntityFrameworkCore;

namespace Dhexby.LogisticsEngine.Infrastructure.Persistence;

/// <summary>Repository di Road su PostgreSQL (tabella "roads").</summary>
internal sealed class PostgresRoadRepository : IRoadRepository
{
    private readonly LogisticsDbContext _context;

    public PostgresRoadRepository(LogisticsDbContext context)
    {
        _context = context;
    }

    public async Task<Road?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        RoadDocument? document = await _context.Roads
            .AsNoTracking()
            .FirstOrDefaultAsync(document => document.Id == id, cancellationToken);

        return document?.ToDomain();
    }

    public Task<bool> ExistsBetweenAsync(
        Guid firstHubId,
        Guid secondHubId,
        CancellationToken cancellationToken = default) =>
        _context.Roads
            .AsNoTracking()
            .AnyAsync(
                document =>
                    (document.FromHubId == firstHubId && document.ToHubId == secondHubId) ||
                    (document.FromHubId == secondHubId && document.ToHubId == firstHubId),
                cancellationToken);

    public async Task<IReadOnlyList<Road>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        List<RoadDocument> documents = await _context.Roads
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return documents.Select(document => document.ToDomain()).ToArray();
    }

    public async Task AddAsync(Road road, CancellationToken cancellationToken = default) =>
        await _context.Roads.AddAsync(RoadDocument.FromDomain(road), cancellationToken);
}
