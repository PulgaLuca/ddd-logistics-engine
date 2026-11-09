using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using LogisticsEngine.Application.Users;
using LogisticsEngine.Domain.Users.Enums;

namespace LogisticsEngine.Infrastructure.Users;

/// <summary>Emette e verifica token compatti firmati HMAC-SHA256 per la demo locale.</summary>
public sealed class HmacUserAccessTokenService : IUserAccessTokenService, IUserAccessTokenValidator
{
    private static readonly TimeSpan _tokenLifetime = TimeSpan.FromHours(1);
    private const string _issuer = "LogisticsEngine";
    private const string _audience = "LogisticsEngine.Api";
    private const string _headerJson = "{\"alg\":\"HS256\",\"typ\":\"JWT\"}";
    private readonly byte[] _signingKey;

    public HmacUserAccessTokenService(string signingKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(signingKey);
        _signingKey = Encoding.UTF8.GetBytes(signingKey);
        if (_signingKey.Length < 32)
        {
            throw new ArgumentException("La chiave di firma deve contenere almeno 32 byte.", nameof(signingKey));
        }
    }

    public UserAccessToken Create(UserDto user)
    {
        DateTimeOffset expiresAt = DateTimeOffset.FromUnixTimeSeconds(
            DateTimeOffset.UtcNow.Add(_tokenLifetime).ToUnixTimeSeconds());
        byte[] payload = JsonSerializer.SerializeToUtf8Bytes(new TokenPayload(
            user.Id.ToString(), user.UserName, user.Role, _issuer, _audience, expiresAt.ToUnixTimeSeconds()));
        string unsignedToken = $"{Base64UrlEncode(Encoding.UTF8.GetBytes(_headerJson))}.{Base64UrlEncode(payload)}";
        byte[] signature = HMACSHA256.HashData(_signingKey, Encoding.ASCII.GetBytes(unsignedToken));

        return new UserAccessToken($"{unsignedToken}.{Base64UrlEncode(signature)}", expiresAt);
    }

    public UserTokenClaims? Validate(string token)
    {
        string[] parts = token.Split('.');
        if (parts.Length != 3)
        {
            return null;
        }

        try
        {
            string unsignedToken = $"{parts[0]}.{parts[1]}";
            byte[] expectedSignature = HMACSHA256.HashData(_signingKey, Encoding.ASCII.GetBytes(unsignedToken));
            byte[] providedSignature = Base64UrlDecode(parts[2]);
            if (!CryptographicOperations.FixedTimeEquals(expectedSignature, providedSignature))
            {
                return null;
            }

            using JsonDocument header = JsonDocument.Parse(Base64UrlDecode(parts[0]));
            if (header.RootElement.GetProperty("alg").GetString() != "HS256")
            {
                return null;
            }

            using JsonDocument payload = JsonDocument.Parse(Base64UrlDecode(parts[1]));
            JsonElement root = payload.RootElement;
            if (root.GetProperty("iss").GetString() != _issuer ||
                root.GetProperty("aud").GetString() != _audience ||
                !Guid.TryParse(root.GetProperty("sub").GetString(), out Guid userId) ||
                !Enum.TryParse(root.GetProperty("role").GetString(), out UserRole role) ||
                !Enum.IsDefined(role))
            {
                return null;
            }

            DateTimeOffset expiresAt = DateTimeOffset.FromUnixTimeSeconds(root.GetProperty("exp").GetInt64());
            string userName = root.GetProperty("name").GetString() ?? string.Empty;
            return expiresAt > DateTimeOffset.UtcNow && userId != Guid.Empty && !string.IsNullOrWhiteSpace(userName)
                ? new UserTokenClaims(userId, userName, role, expiresAt)
                : null;
        }
        catch (Exception exception) when (exception is FormatException or JsonException or InvalidOperationException or KeyNotFoundException or ArgumentOutOfRangeException)
        {
            return null;
        }
    }

    private static string Base64UrlEncode(byte[] value) =>
        Convert.ToBase64String(value).TrimEnd('=').Replace('+', '-').Replace('/', '_');

    private static byte[] Base64UrlDecode(string value)
    {
        string base64 = value.Replace('-', '+').Replace('_', '/');
        base64 = base64.PadRight(base64.Length + (4 - base64.Length % 4) % 4, '=');
        return Convert.FromBase64String(base64);
    }

    private sealed record TokenPayload(
        [property: JsonPropertyName("sub")] string Subject,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("role")] string Role,
        [property: JsonPropertyName("iss")] string Issuer,
        [property: JsonPropertyName("aud")] string Audience,
        [property: JsonPropertyName("exp")] long ExpiresAt);
}
