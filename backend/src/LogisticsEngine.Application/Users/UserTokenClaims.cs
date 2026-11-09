using LogisticsEngine.Domain.Users.Enums;

namespace LogisticsEngine.Application.Users;

/// <summary>Claim essenziali estratti solo da un token con firma verificata.</summary>
public sealed record UserTokenClaims(Guid UserId, string UserName, UserRole Role, DateTimeOffset ExpiresAtUtc);
