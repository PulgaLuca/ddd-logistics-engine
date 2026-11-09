using LogisticsEngine.Domain.Users.Entities;

namespace LogisticsEngine.Application.Users;

/// <summary>Verifica una credenziale Google e recupera o registra l'utente locale associato.</summary>
public interface IGoogleAuthenticationService
{
    Task<GoogleAuthenticatedUser?> AuthenticateAsync(string? credential, CancellationToken cancellationToken);
}

public sealed record GoogleAuthenticatedUser(User User, string DisplayName);
