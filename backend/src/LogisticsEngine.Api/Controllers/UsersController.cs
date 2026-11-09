using System.Security.Claims;
using LogisticsEngine.Api.Authentication;
using LogisticsEngine.Api.Contracts;
using LogisticsEngine.Application.Users;
using LogisticsEngine.Application.Users.AuthenticateUser;
using LogisticsEngine.Application.Users.AuthorizeUser;
using LogisticsEngine.Application.Users.RegisterUser;
using LogisticsEngine.Application.Users.GetUserProfile;
using LogisticsEngine.Application.Users.UpdateUserProfile;
using LogisticsEngine.Domain.Users.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogisticsEngine.Api.Controllers;

/// <summary>Espone registrazione, login e controllo dei permessi dell'utente autenticato.</summary>
[Route(ApiRouteConstants.VersionOnePrefix + "/users")]
public sealed class UsersController(
    ISender sender,
    IUserAccessTokenService accessTokenService,
    IGoogleAuthenticationService googleAuthenticationService) : ApiControllerBase
{
    /// <summary>Crea un account Viewer; il caso d'uso non accetta ruoli dal client.</summary>
    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType<UserDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new RegisterUserCommand(request.UserName, request.Password), cancellationToken);

        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, result.Value)
            : ToProblem(result.Error);
    }

    /// <summary>Verifica il credential Google e restituisce un token bearer dell'applicazione.</summary>
    [AllowAnonymous]
    [HttpPost("google")]
    [ProducesResponseType<AuthenticateUserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AuthenticateWithGoogle(
        [FromBody] GoogleAuthenticationRequest request,
        CancellationToken cancellationToken)
    {
        GoogleAuthenticatedUser? authenticated = await googleAuthenticationService.AuthenticateAsync(request.Credential, cancellationToken);
        if (authenticated is null)
        {
            return Unauthorized(new ProblemDetails { Title = "Google authentication failed", Detail = "Il credential Google non è valido o il provider non è configurato." });
        }
        UserDto user = UserDto.From(authenticated.User, authenticated.DisplayName);
        UserAccessToken token = accessTokenService.Create(user);
        return Ok(new AuthenticateUserResponse(token.Value, "Bearer", token.ExpiresAtUtc, user));
    }

    /// <summary>Restituisce il profilo dell'utente autenticato.</summary>
    [Authorize(AuthenticationSchemes = BearerTokenAuthenticationDefaults.Scheme)]
    [HttpGet("me")]
    [ProducesResponseType<UserDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyProfile(CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId))
        {
            return Unauthorized();
        }
        var result = await sender.Send(new GetUserProfileQuery(userId), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : ToProblem(result.Error);
    }

    /// <summary>Aggiorna nome, cognome e ruolo professionale del solo utente autenticato.</summary>
    [Authorize(AuthenticationSchemes = BearerTokenAuthenticationDefaults.Scheme)]
    [HttpPut("me")]
    [ProducesResponseType<UserDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateMyProfile(
        [FromBody] UpdateUserProfileRequest request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid userId))
        {
            return Unauthorized();
        }
        var result = await sender.Send(new UpdateUserProfileCommand(
            userId, request.FirstName, request.LastName, request.ProfessionalRole), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : ToProblem(result.Error);
    }

    /// <summary>Verifica le credenziali e restituisce un token bearer a scadenza.</summary>
    [AllowAnonymous]
    [HttpPost("authenticate")]
    [ProducesResponseType<AuthenticateUserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Authenticate(
        [FromBody] AuthenticateUserRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new AuthenticateUserCommand(request.UserName, request.Password), cancellationToken);
        if (result.IsFailure)
        {
            return ToProblem(result.Error);
        }

        UserAccessToken token = accessTokenService.Create(result.Value);
        return Ok(new AuthenticateUserResponse(token.Value, "Bearer", token.ExpiresAtUtc, result.Value));
    }

    /// <summary>Controlla il permesso dell'utente identificato dal token, mai da un ID fornito dal client.</summary>
    [Authorize(AuthenticationSchemes = BearerTokenAuthenticationDefaults.Scheme)]
    [HttpGet("me/permissions/{permission}")]
    [ProducesResponseType<PermissionCheckResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CheckPermission(
        UserPermission permission,
        CancellationToken cancellationToken)
    {
        string? userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdValue, out Guid userId))
        {
            return Unauthorized();
        }

        var result = await sender.Send(new AuthorizeUserQuery(userId, permission), cancellationToken);
        return result.IsSuccess
            ? Ok(new PermissionCheckResponse(permission, result.Value))
            : ToProblem(result.Error);
    }
}

/// <summary>Esito del controllo del permesso richiesto.</summary>
public sealed record PermissionCheckResponse(UserPermission Permission, bool Allowed);

/// <summary>Credential ID token prodotto da Google Identity Services.</summary>
public sealed record GoogleAuthenticationRequest(string? Credential);
