namespace LogisticsEngine.Api.Contracts;

/// <summary>Credenziali richieste per creare un nuovo account.</summary>
public sealed record RegisterUserRequest(string? UserName, string? Password);

/// <summary>Credenziali inviate per accedere.</summary>
public sealed record AuthenticateUserRequest(string? UserName, string? Password);

/// <summary>Dati modificabili del profilo personale; il ruolo di autorizzazione non è modificabile qui.</summary>
public sealed record UpdateUserProfileRequest(string? FirstName, string? LastName, string? ProfessionalRole);

/// <summary>Risposta di login: token bearer e dati pubblici dell'utente.</summary>
public sealed record AuthenticateUserResponse(
    string AccessToken,
    string TokenType,
    DateTimeOffset ExpiresAtUtc,
    LogisticsEngine.Application.Users.UserDto User);
