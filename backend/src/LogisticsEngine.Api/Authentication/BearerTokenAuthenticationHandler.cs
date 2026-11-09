using System.Security.Claims;
using System.Text.Encodings.Web;
using LogisticsEngine.Application.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace LogisticsEngine.Api.Authentication;

/// <summary>Valida token Bearer firmati dall'app e crea l'identità usata da [Authorize].</summary>
public sealed class BearerTokenAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IUserAccessTokenValidator tokenValidator)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string? authorization = Request.Headers.Authorization.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(authorization) ||
            !authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        string token = authorization["Bearer ".Length..].Trim();
        UserTokenClaims? tokenClaims = tokenValidator.Validate(token);
        if (tokenClaims is null)
        {
            return Task.FromResult(AuthenticateResult.Fail("Token bearer non valido o scaduto."));
        }

        Claim[] claims =
        [
            new(ClaimTypes.NameIdentifier, tokenClaims.UserId.ToString()),
            new(ClaimTypes.Name, tokenClaims.UserName),
            new(ClaimTypes.Role, tokenClaims.Role.ToString())
        ];
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
