using LogisticsEngine.SharedKernel;
using MediatR;

namespace LogisticsEngine.Application.Users.AuthenticateUser;

/// <summary>Verifica le credenziali inserite; gli errori non rivelano se l'account esiste.</summary>
public sealed record AuthenticateUserCommand(string? UserName, string? Password) : IRequest<Result<UserDto>>;
