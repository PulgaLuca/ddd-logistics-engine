using Dhexby.LogisticsEngine.Domain.Users.Events;
using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Users;

/// <summary>
/// Aggregate root dell'utente autenticabile. La password non è mai in chiaro:
/// l'entità conserva solo l'hash (calcolato in Infrastructure). Il ruolo guida
/// l'autorizzazione lato API.
/// </summary>
public sealed class User : Entity
{
    private User(Guid id, Email email, string passwordHash, UserRole role, DateTime createdOnUtc)
        : base(id)
    {
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        CreatedOnUtc = createdOnUtc;
    }

    public Email Email { get; private set; }

    public string PasswordHash { get; private set; }

    public UserRole Role { get; private set; }

    public DateTime CreatedOnUtc { get; private set; }

    /// <summary>Factory: unico punto di creazione di un utente registrato.</summary>
    public static Result<User> Create(Email email, string passwordHash, UserRole role, DateTime utcNow)
    {
        ArgumentNullException.ThrowIfNull(email);

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            return Result.Failure<User>(UserErrors.InvalidCredentials);
        }

        var user = new User(Guid.NewGuid(), email, passwordHash, role, utcNow);
        user.RaiseDomainEvent(new UserRegisteredDomainEvent(user.Id, role));

        return user;
    }

    /// <summary>Ricostruisce l'aggregato dallo store senza eventi né regole.</summary>
    public static User Rehydrate(Guid id, Email email, string passwordHash, UserRole role, DateTime createdOnUtc) =>
        new(id, email, passwordHash, role, createdOnUtc);
}
