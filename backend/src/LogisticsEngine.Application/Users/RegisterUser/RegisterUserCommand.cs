using LogisticsEngine.SharedKernel;
using MediatR;

namespace LogisticsEngine.Application.Users.RegisterUser;

/// <summary>Richiesta interna per registrare un utente; il ruolo iniziale è sempre Viewer.</summary>
public sealed record RegisterUserCommand(string? UserName, string? Password) : IRequest<Result<UserDto>>;
