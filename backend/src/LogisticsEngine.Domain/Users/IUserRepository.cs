using LogisticsEngine.Domain.Users.Entities;
using LogisticsEngine.Domain.Users.ValueObjects;

namespace LogisticsEngine.Domain.Users;

/// <summary>Porta del Domain per conservare e recuperare gli utenti.</summary>
public interface IUserRepository
{
    User? GetById(Guid id);
    User? GetByUserName(UserName userName);
    void Add(User user);
    void Update(User user);
}
