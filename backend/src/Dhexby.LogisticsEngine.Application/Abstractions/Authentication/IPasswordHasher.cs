namespace Dhexby.LogisticsEngine.Application.Abstractions.Authentication;

/// <summary>
/// Astrazione dell'hashing delle password. L'implementazione concreta (con
/// salt e algoritmo sicuro) vive in Infrastructure.
/// </summary>
public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(string passwordHash, string password);
}
