using Dhexby.LogisticsEngine.Domain.Users;

namespace Dhexby.LogisticsEngine.Application.Abstractions.Authentication;

/// <summary>Genera il token di accesso (JWT) per un utente autenticato.</summary>
public interface IJwtTokenGenerator
{
    AccessToken Generate(User user);
}

/// <summary>Token firmato e relativa scadenza.</summary>
public sealed record AccessToken(string Token, DateTime ExpiresAtUtc);
