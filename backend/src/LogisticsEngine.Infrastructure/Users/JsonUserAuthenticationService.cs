using LogisticsEngine.Application.Users;
using LogisticsEngine.Domain.Users;
using LogisticsEngine.Domain.Users.Entities;
using LogisticsEngine.Domain.Users.ValueObjects;
using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.Infrastructure.Users;

/// <summary>Autentica leggendo l'utente dal repository e confrontando la password con il suo hash.</summary>
public sealed class JsonUserAuthenticationService(IUserRepository repository, IPasswordHasher passwordHasher)
    : IUserAuthenticationService
{
    public Result<User> Authenticate(string? userName, string? password)
    {
        Result<UserName> parsedName = UserName.Create(userName);
        if (parsedName.IsFailure || string.IsNullOrEmpty(password))
        {
            return Result.Failure<User>(UserErrors.InvalidCredentials);
        }

        // Il repository JSON rilegge users.json per ogni autenticazione; il confronto usa il hasher configurato.
        User? user = repository.GetByUserName(parsedName.Value);
        return user is not null && passwordHasher.Verify(password, user.PasswordHash.Value)
            ? Result.Success(user)
            : Result.Failure<User>(UserErrors.InvalidCredentials);
    }
}
