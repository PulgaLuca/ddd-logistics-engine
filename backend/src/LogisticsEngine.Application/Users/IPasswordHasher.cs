namespace LogisticsEngine.Application.Users;

/// <summary>Porta per trasformare password in hash e verificarle senza esporre l'algoritmo ai casi d'uso.</summary>
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string passwordHash);
}
