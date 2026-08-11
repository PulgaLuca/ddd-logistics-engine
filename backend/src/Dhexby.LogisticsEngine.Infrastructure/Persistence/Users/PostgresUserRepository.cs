using Dhexby.LogisticsEngine.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Dhexby.LogisticsEngine.Infrastructure.Persistence;

/// <summary>Repository di User su PostgreSQL (tabella "users").</summary>
internal sealed class PostgresUserRepository : IUserRepository
{
    private readonly LogisticsDbContext _context;

    public PostgresUserRepository(LogisticsDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        UserDocument? document = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(document => document.Id == id, cancellationToken);

        return document?.ToDomain();
    }

    public async Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(email);

        UserDocument? document = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(document => document.Email == email.Value, cancellationToken);

        return document?.ToDomain();
    }

    public Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default) =>
        _context.Users
            .AsNoTracking()
            .AnyAsync(document => document.Email == email.Value, cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken = default) =>
        await _context.Users.AddAsync(UserDocument.FromDomain(user), cancellationToken);
}
