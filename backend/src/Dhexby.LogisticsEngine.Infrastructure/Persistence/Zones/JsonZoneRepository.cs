using Dhexby.LogisticsEngine.Domain.Zones;
using Microsoft.Extensions.Options;

namespace Dhexby.LogisticsEngine.Infrastructure.Persistence;

/// <summary>Repository di Zone su file JSON, basato sulla base condivisa.</summary>
internal sealed class JsonZoneRepository : JsonEntityRepository<ZoneDocument>, IZoneRepository
{
    public JsonZoneRepository(IOptions<JsonStorageOptions> options)
        : base(options.Value.ZonesFilePath)
    {
    }

    public Task<Zone?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        QueryAsync(documents => documents.Find(document => document.Id == id)?.ToDomain(), cancellationToken);

    public Task<bool> ExistsByCodeAsync(ZoneCode code, CancellationToken cancellationToken = default) =>
        QueryAsync(documents => documents.Exists(document => document.Code == code.Value), cancellationToken);

    public Task<IReadOnlyList<Zone>> GetChildrenAsync(
        Guid parentZoneId,
        CancellationToken cancellationToken = default) =>
        QueryAsync<IReadOnlyList<Zone>>(
            documents => documents
                .Where(document => document.ParentZoneId == parentZoneId)
                .Select(document => document.ToDomain())
                .ToArray(),
            cancellationToken);

    public Task AddAsync(Zone zone, CancellationToken cancellationToken = default) =>
        MutateAsync(documents => documents.Add(ZoneDocument.FromDomain(zone)), cancellationToken);

    public Task UpdateAsync(Zone zone, CancellationToken cancellationToken = default) =>
        MutateAsync(
            documents =>
            {
                int index = documents.FindIndex(document => document.Id == zone.Id);
                if (index < 0)
                {
                    throw new InvalidOperationException(
                        $"Cannot update zone '{zone.Id}': not found in the JSON store.");
                }

                documents[index] = ZoneDocument.FromDomain(zone);
            },
            cancellationToken);
}
