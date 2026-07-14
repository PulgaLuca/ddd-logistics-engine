using Dhexby.LogisticsEngine.Domain.Users;
using Microsoft.Extensions.Options;

namespace Dhexby.LogisticsEngine.Infrastructure.Persistence;

/// <summary>Repository di User su file JSON, basato sulla base condivisa.</summary>
internal sealed class JsonUserRepository : JsonEntityRepository<UserDocument>, IUserRepository
{
    public JsonUserRepository(IOptions<JsonStorageOptions> options)
        : base(options.Value.UsersFilePath)
    {
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        QueryAsync(documents => documents.Find(document => document.Id == id)?.ToDomain(), cancellationToken);

    public Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default) =>
        QueryAsync(documents => documents.Find(document => document.Email == email.Value)?.ToDomain(), cancellationToken);

    public Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default) =>
        QueryAsync(documents => documents.Exists(document => document.Email == email.Value), cancellationToken);

    public Task AddAsync(User user, CancellationToken cancellationToken = default) =>
        MutateAsync(documents => documents.Add(UserDocument.FromDomain(user)), cancellationToken);
}
