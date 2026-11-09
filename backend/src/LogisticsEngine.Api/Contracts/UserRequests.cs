namespace LogisticsEngine.Api.Contracts;

/// <summary>Credenziali richieste per creare un nuovo account.</summary>
public sealed record RegisterUserRequest(string? UserName, string? Password);

/// <summary>Credenziali inviate per accedere.</summary>
public sealed record AuthenticateUserRequest(string? UserName, string? Password);

/// <summary>Risposta di login: token bearer e dati pubblici dell'utente.</summary>
public sealed record AuthenticateUserResponse(
    string AccessToken,
    string TokenType,
    DateTimeOffset ExpiresAtUtc,
    LogisticsEngine.Application.Users.UserDto User);
