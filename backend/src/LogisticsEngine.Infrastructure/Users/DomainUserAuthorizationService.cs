using LogisticsEngine.Application.Users;
using LogisticsEngine.Domain.Users;
using LogisticsEngine.Domain.Users.Enums;
using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.Infrastructure.Users;

/// <summary>Recupera l'utente dal repository e delega la decisione sui permessi al modello di dominio.</summary>
public sealed class DomainUserAuthorizationService(IUserRepository repository) : IUserAuthorizationService
{
    public Result<bool> Authorize(Guid userId, UserPermission permission)
    {
        var user = repository.GetById(userId);
        if (user is null)
        {
            return Result.Failure<bool>(UserErrors.UserNotFound);
        }

        return user.HasPermission(permission)
            ? Result.Success(true)
            : Result.Failure<bool>(UserErrors.PermissionDenied);
    }
}
