using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Users;

/// <summary>Catalogo centralizzato (DRY) degli errori di dominio dell'utente.</summary>
public static class UserErrors
{
    public static readonly Error EmailEmpty = Error.Failure(
        "User.Email.Empty",
        "The email is required.");

    public static readonly Error EmailInvalidFormat = Error.Failure(
        "User.Email.InvalidFormat",
        "The email format is not valid.");

    public static readonly Error EmailAlreadyInUse = Error.Conflict(
        "User.Email.AlreadyInUse",
        "An account with the same email already exists.");

    public static readonly Error UnknownRole = Error.Failure(
        "User.Role.Unknown",
        "The role is not a valid value.");

    // Messaggio volutamente generico: non rivela se sia sbagliata l'email o la password.
    public static readonly Error InvalidCredentials = Error.Unauthorized(
        "User.InvalidCredentials",
        "The email or password is incorrect.");

    public static Error NotFound(Guid userId) => Error.NotFound(
        "User.NotFound",
        $"The user with id '{userId}' was not found.");
}
