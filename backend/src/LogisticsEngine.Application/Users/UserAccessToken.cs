namespace LogisticsEngine.Application.Users;

/// <summary>Token da restituire al client dopo credenziali verificate.</summary>
public sealed record UserAccessToken(string Value, DateTimeOffset ExpiresAtUtc);
