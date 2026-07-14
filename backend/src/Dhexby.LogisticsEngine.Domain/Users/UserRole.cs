namespace Dhexby.LogisticsEngine.Domain.Users;

/// <summary>
/// Ruolo dell'utente, dal più al meno privilegiato. Determina cosa può fare
/// tramite l'autorizzazione dell'API.
/// </summary>
public enum UserRole
{
    Administrator = 0,
    Manager = 1,
    Operator = 2,
    Viewer = 3
}
