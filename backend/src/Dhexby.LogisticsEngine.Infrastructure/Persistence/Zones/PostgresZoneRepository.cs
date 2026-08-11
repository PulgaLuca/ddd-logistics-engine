using Dhexby.LogisticsEngine.Domain.Zones;
using Microsoft.EntityFrameworkCore;

namespace Dhexby.LogisticsEngine.Infrastructure.Persistence;

/// <summary>Repository di Zone su PostgreSQL (tabella "zones").</summary>
internal sealed class PostgresZoneRepository : IZoneRepository
{
    private readonly LogisticsDbContext _context;

    public PostgresZoneRepository(LogisticsDbContext context)
    {
        _context = context;
    }

    public async Task<Zone?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        ZoneDocument? document = await _context.Zones
            .AsNoTracking()
            .FirstOrDefaultAsync(document => document.Id == id, cancellationToken);

        return document?.ToDomain();
    }

    public Task<bool> ExistsByCodeAsync(ZoneCode code, CancellationToken cancellationToken = default) =>
        _context.Zones
            .AsNoTracking()
            .AnyAsync(document => document.Code == code.Value, cancellationToken);

    public async Task<IReadOnlyList<Zone>> GetChildrenAsync(
        Guid parentZoneId,
        CancellationToken cancellationToken = default)
    {
        List<ZoneDocument> documents = await _context.Zones
            .AsNoTracking()
            .Where(document => document.ParentZoneId == parentZoneId)
            .ToListAsync(cancellationToken);

        return documents.Select(document => document.ToDomain()).ToArray();
    }

    public async Task AddAsync(Zone zone, CancellationToken cancellationToken = default) =>
        await _context.Zones.AddAsync(ZoneDocument.FromDomain(zone), cancellationToken);

    public async Task UpdateAsync(Zone zone, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(zone);

        bool exists = await _context.Zones
            .AsNoTracking()
            .AnyAsync(document => document.Id == zone.Id, cancellationToken);

        if (!exists)
        {
            throw new InvalidOperationException($"Cannot update zone '{zone.Id}': not found in PostgreSQL.");
        }

        _context.Zones.Update(ZoneDocument.FromDomain(zone));
    }
}
