using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.Domain.Users;

public static class UserErrors
{
    public static readonly Error InvalidId = new("User.InvalidId", "L'identificativo utente non è valido.", ErrorType.Validation);
    public static readonly Error InvalidUserName = new("User.InvalidUserName", "Il nome utente deve contenere 3-64 caratteri alfanumerici, '.', '_' o '-'.", ErrorType.Validation);
    public static readonly Error InvalidPasswordHash = new("User.InvalidPasswordHash", "L'hash della password non è valido.", ErrorType.Validation);
    public static readonly Error InvalidRole = new("User.InvalidRole", "Il ruolo utente non è valido.", ErrorType.Validation);
    public static readonly Error DuplicateUserName = new("User.DuplicateUserName", "Il nome utente è già registrato.", ErrorType.Conflict);
    public static readonly Error InvalidCredentials = new("User.InvalidCredentials", "Nome utente o password non validi.", ErrorType.Unauthorized);
    public static readonly Error UserNotFound = new("User.NotFound", "L'utente richiesto non esiste.", ErrorType.NotFound);
    public static readonly Error PermissionDenied = new("User.PermissionDenied", "L'utente non possiede il permesso richiesto.", ErrorType.Forbidden);
    public static readonly Error InvalidPassword = new("User.InvalidPassword", "La password deve contenere almeno 12 caratteri.", ErrorType.Validation);
    public static readonly Error InvalidProfile = new("User.InvalidProfile", "Nome e cognome sono obbligatori e i campi non possono superare 80 caratteri.", ErrorType.Validation);
}
