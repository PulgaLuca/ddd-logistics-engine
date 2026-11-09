using LogisticsEngine.Domain.Users.Entities;
using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.Application.Users;

/// <summary>Porta per autenticare credenziali; l'implementazione sceglie archivio e algoritmo di verifica.</summary>
public interface IUserAuthenticationService
{
    Result<User> Authenticate(string? userName, string? password);
}
