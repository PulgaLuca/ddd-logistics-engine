using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dhexby.LogisticsEngine.Application.Abstractions.Authentication;
using Dhexby.LogisticsEngine.Domain.Users;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Dhexby.LogisticsEngine.Infrastructure.Authentication;

/// <summary>
/// Genera un JWT firmato (HMAC-SHA256) con i claim identità/ruolo dell'utente.
/// La chiave e i parametri arrivano dalla configurazione (sezione "Jwt").
/// </summary>
internal sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtOptions _options;

    public JwtTokenGenerator(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public AccessToken Generate(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        DateTime expiresAtUtc = DateTime.UtcNow.AddMinutes(_options.ExpiryMinutes);

        Claim[] claims =
        [
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email.Value),
            new(ClaimTypes.Role, user.Role.ToString()),
        ];

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

        return new AccessToken(tokenValue, expiresAtUtc);
    }
}
