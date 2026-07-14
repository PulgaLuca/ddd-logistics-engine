using Dhexby.LogisticsEngine.Domain.Hubs;
using Microsoft.Extensions.Options;

namespace Dhexby.LogisticsEngine.Infrastructure.Persistence;

/// <summary>Repository di Hub su file JSON, basato sulla base condivisa.</summary>
internal sealed class JsonHubRepository : JsonEntityRepository<HubDocument>, IHubRepository
{
    public JsonHubRepository(IOptions<JsonStorageOptions> options)
        : base(options.Value.HubsFilePath)
    {
    }

    public Task<Hub?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        QueryAsync(documents => documents.Find(document => document.Id == id)?.ToDomain(), cancellationToken);

    public Task<bool> ExistsByCodeAsync(HubCode code, CancellationToken cancellationToken = default) =>
        QueryAsync(documents => documents.Exists(document => document.Code == code.Value), cancellationToken);

    public Task<IReadOnlyList<Hub>> GetAllAsync(CancellationToken cancellationToken = default) =>
        QueryAsync<IReadOnlyList<Hub>>(
            documents => documents.Select(document => document.ToDomain()).ToArray(),
            cancellationToken);

    public Task AddAsync(Hub hub, CancellationToken cancellationToken = default) =>
        MutateAsync(documents => documents.Add(HubDocument.FromDomain(hub)), cancellationToken);

    public Task UpdateAsync(Hub hub, CancellationToken cancellationToken = default) =>
        MutateAsync(
            documents =>
            {
                int index = documents.FindIndex(document => document.Id == hub.Id);
                if (index < 0)
                {
                    throw new InvalidOperationException(
                        $"Cannot update hub '{hub.Id}': not found in the JSON store.");
                }

                documents[index] = HubDocument.FromDomain(hub);
            },
            cancellationToken);
}
