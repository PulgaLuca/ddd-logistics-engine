using Dhexby.LogisticsEngine.Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Identity;

namespace Dhexby.LogisticsEngine.Infrastructure.Authentication;

/// <summary>
/// Hashing delle password basato su <see cref="PasswordHasher{TUser}"/> di ASP.NET
/// (PBKDF2 con salt per hash): robusto e senza dipendere da Entity Framework.
/// </summary>
internal sealed class Pbkdf2PasswordHasher : IPasswordHasher
{
    private static readonly object _placeholderUser = new();
    private readonly PasswordHasher<object> _hasher = new();

    public string Hash(string password) => _hasher.HashPassword(_placeholderUser, password);

    public bool Verify(string passwordHash, string password)
    {
        PasswordVerificationResult result = _hasher.VerifyHashedPassword(_placeholderUser, passwordHash, password);

        return result != PasswordVerificationResult.Failed;
    }
}
