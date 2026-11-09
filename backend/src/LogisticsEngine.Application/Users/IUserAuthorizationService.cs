using LogisticsEngine.Domain.Users.Enums;
using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.Application.Users;

/// <summary>Porta per verificare un permesso del Domain associato a un utente.</summary>
public interface IUserAuthorizationService
{
    Result<bool> Authorize(Guid userId, UserPermission permission);
}
