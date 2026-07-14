using Dhexby.LogisticsEngine.Domain.Users;
using MongoDB.Driver;

namespace Dhexby.LogisticsEngine.Infrastructure.Persistence;

/// <summary>Repository di User su MongoDB (collection "users").</summary>
internal sealed class MongoUserRepository : IUserRepository
{
    private readonly IMongoCollection<UserDocument> _collection;

    public MongoUserRepository(MongoDbContext context)
    {
        _collection = context.GetCollection<UserDocument>(MongoDbContext.UsersCollection);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        UserDocument? document = await _collection
            .Find(document => document.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        return document?.ToDomain();
    }

    public async Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        UserDocument? document = await _collection
            .Find(document => document.Email == email.Value)
            .FirstOrDefaultAsync(cancellationToken);

        return document?.ToDomain();
    }

    public Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default) =>
        _collection.Find(document => document.Email == email.Value).AnyAsync(cancellationToken);

    public Task AddAsync(User user, CancellationToken cancellationToken = default) =>
        _collection.InsertOneAsync(UserDocument.FromDomain(user), options: null, cancellationToken);
}
