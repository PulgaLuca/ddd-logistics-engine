namespace Dhexby.LogisticsEngine.Application.Users;

/// <summary>
/// Esito di registrazione/login: il token da usare come Bearer, la scadenza e
/// i dati essenziali dell'utente. Forma stabile per il client.
/// </summary>
public sealed record AuthResponse(
    string AccessToken,
    DateTime ExpiresAtUtc,
    string Email,
    string Role);
