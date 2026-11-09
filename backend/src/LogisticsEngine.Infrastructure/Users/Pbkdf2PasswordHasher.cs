using System.Globalization;
using System.Security.Cryptography;
using LogisticsEngine.Application.Users;

namespace LogisticsEngine.Infrastructure.Users;

/// <summary>Hash PBKDF2 con salt casuale, parametri incorporati nel formato e confronto a tempo costante.</summary>
public sealed class Pbkdf2PasswordHasher : IPasswordHasher
{
    private const string _algorithm = "pbkdf2-sha256";
    private const int _iterationCount = 600_000;
    private const int _saltLength = 16;
    private const int _hashLength = 32;

    public string Hash(string password)
    {
        ArgumentException.ThrowIfNullOrEmpty(password);

        byte[] salt = RandomNumberGenerator.GetBytes(_saltLength);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password, salt, _iterationCount, HashAlgorithmName.SHA256, _hashLength);

        return string.Join("$", [
            _algorithm,
            _iterationCount.ToString(CultureInfo.InvariantCulture),
            Convert.ToBase64String(salt),
            Convert.ToBase64String(hash)
        ]);
    }

    public bool Verify(string password, string passwordHash)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(passwordHash))
        {
            return false;
        }

        string[] parts = passwordHash.Split('$');
        if (parts.Length != 4 || parts[0] != _algorithm ||
            !int.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out int iterations) ||
            iterations is < 100_000 or > 1_000_000)
        {
            return false;
        }

        try
        {
            byte[] salt = Convert.FromBase64String(parts[2]);
            byte[] expectedHash = Convert.FromBase64String(parts[3]);
            if (salt.Length != _saltLength || expectedHash.Length != _hashLength)
            {
                return false;
            }

            byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
                password, salt, iterations, HashAlgorithmName.SHA256, expectedHash.Length);
            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
