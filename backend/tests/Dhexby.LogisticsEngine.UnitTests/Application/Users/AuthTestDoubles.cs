using Dhexby.LogisticsEngine.Application.Abstractions.Authentication;
using Dhexby.LogisticsEngine.Domain.Users;

namespace Dhexby.LogisticsEngine.UnitTests.Application.Users;

/// <summary>Fake in-memory di <see cref="IUserRepository"/>.</summary>
internal sealed class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users = [];

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(_users.Find(user => user.Id == id));

    public Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default) =>
        Task.FromResult(_users.Find(user => user.Email.Equals(email)));

    public Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default) =>
        Task.FromResult(_users.Exists(user => user.Email.Equals(email)));

    public Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        _users.Add(user);
        return Task.CompletedTask;
    }
}

/// <summary>Hasher fittizio deterministico: "hashed:" + password.</summary>
internal sealed class FakePasswordHasher : IPasswordHasher
{
    public string Hash(string password) => $"hashed:{password}";

    public bool Verify(string passwordHash, string password) => passwordHash == $"hashed:{password}";
}

/// <summary>Generatore di token fittizio.</summary>
internal sealed class FakeJwtTokenGenerator : IJwtTokenGenerator
{
    public AccessToken Generate(User user) =>
        new($"token-for-{user.Email.Value}", DateTime.UtcNow.AddHours(1));
}
