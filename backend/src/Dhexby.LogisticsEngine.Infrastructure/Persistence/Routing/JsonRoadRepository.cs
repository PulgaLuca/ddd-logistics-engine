using Dhexby.LogisticsEngine.Domain.Routing;
using Microsoft.Extensions.Options;

namespace Dhexby.LogisticsEngine.Infrastructure.Persistence;

/// <summary>Repository di Road su file JSON, basato sulla base condivisa.</summary>
internal sealed class JsonRoadRepository : JsonEntityRepository<RoadDocument>, IRoadRepository
{
    public JsonRoadRepository(IOptions<JsonStorageOptions> options)
        : base(options.Value.RoadsFilePath)
    {
    }

    public Task<Road?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        QueryAsync(documents => documents.Find(document => document.Id == id)?.ToDomain(), cancellationToken);

    public Task<bool> ExistsBetweenAsync(
        Guid firstHubId,
        Guid secondHubId,
        CancellationToken cancellationToken = default) =>
        QueryAsync(
            documents => documents.Exists(document =>
                (document.FromHubId == firstHubId && document.ToHubId == secondHubId) ||
                (document.FromHubId == secondHubId && document.ToHubId == firstHubId)),
            cancellationToken);

    public Task<IReadOnlyList<Road>> GetAllAsync(CancellationToken cancellationToken = default) =>
        QueryAsync<IReadOnlyList<Road>>(
            documents => documents.Select(document => document.ToDomain()).ToArray(),
            cancellationToken);

    public Task AddAsync(Road road, CancellationToken cancellationToken = default) =>
        MutateAsync(documents => documents.Add(RoadDocument.FromDomain(road)), cancellationToken);
}
