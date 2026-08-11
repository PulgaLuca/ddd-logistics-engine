using Dhexby.LogisticsEngine.Domain.Hubs;
using Microsoft.EntityFrameworkCore;

namespace Dhexby.LogisticsEngine.Infrastructure.Persistence;

/// <summary>Repository di Hub su PostgreSQL (tabella "hubs").</summary>
internal sealed class PostgresHubRepository : IHubRepository
{
    private readonly LogisticsDbContext _context;

    public PostgresHubRepository(LogisticsDbContext context)
    {
        _context = context;
    }

    public async Task<Hub?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        HubDocument? document = await _context.Hubs
            .AsNoTracking()
            .FirstOrDefaultAsync(document => document.Id == id, cancellationToken);

        return document?.ToDomain();
    }

    public Task<bool> ExistsByCodeAsync(HubCode code, CancellationToken cancellationToken = default) =>
        _context.Hubs
            .AsNoTracking()
            .AnyAsync(document => document.Code == code.Value, cancellationToken);

    public async Task<IReadOnlyList<Hub>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        List<HubDocument> documents = await _context.Hubs
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return documents.Select(document => document.ToDomain()).ToArray();
    }

    public async Task AddAsync(Hub hub, CancellationToken cancellationToken = default) =>
        await _context.Hubs.AddAsync(HubDocument.FromDomain(hub), cancellationToken);

    public async Task UpdateAsync(Hub hub, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(hub);

        bool exists = await _context.Hubs
            .AsNoTracking()
            .AnyAsync(document => document.Id == hub.Id, cancellationToken);

        if (!exists)
        {
            throw new InvalidOperationException($"Cannot update hub '{hub.Id}': not found in PostgreSQL.");
        }

        _context.Hubs.Update(HubDocument.FromDomain(hub));
    }
}
