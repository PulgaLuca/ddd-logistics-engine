using Dhexby.LogisticsEngine.Domain.Users;

namespace Dhexby.LogisticsEngine.Infrastructure.Persistence;

/// <summary>Modello di persistenza dell'utente (solo primitivi).</summary>
internal sealed record UserDocument(
    Guid Id,
    string Email,
    string PasswordHash,
    UserRole Role,
    DateTime CreatedOnUtc)
{
    public static UserDocument FromDomain(User user) => new(
        user.Id,
        user.Email.Value,
        user.PasswordHash,
        user.Role,
        user.CreatedOnUtc);

    public User ToDomain() => User.Rehydrate(
        Id,
        Domain.Users.Email.Create(Email).Value,
        PasswordHash,
        Role,
        CreatedOnUtc);
}
