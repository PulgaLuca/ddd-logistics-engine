using Dhexby.LogisticsEngine.Api.Extensions;
using Dhexby.LogisticsEngine.Application.Users;
using Dhexby.LogisticsEngine.Application.Users.Login;
using Dhexby.LogisticsEngine.Application.Users.RegisterUser;
using Dhexby.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dhexby.LogisticsEngine.Api.Controllers;

/// <summary>
/// Endpoint pubblici di autenticazione: registrazione e login. Restituiscono un
/// token JWT da usare come Bearer per chiamare gli endpoint protetti.
/// </summary>
[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public sealed class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>Registra un nuovo utente scegliendo il ruolo e restituisce subito il token.</summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        Result<AuthResponse> result = await _sender.Send(
            new RegisterUserCommand(request.Email, request.Password, request.Role),
            cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblemResult();
    }

    /// <summary>Autentica un utente e restituisce il token.</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        Result<AuthResponse> result = await _sender.Send(
            new LoginCommand(request.Email, request.Password),
            cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblemResult();
    }
}

/// <summary>Contratto HTTP di registrazione (Role: Administrator | Manager | Operator | Viewer).</summary>
public sealed record RegisterUserRequest(string Email, string Password, string Role);

/// <summary>Contratto HTTP di login.</summary>
public sealed record LoginRequest(string Email, string Password);
