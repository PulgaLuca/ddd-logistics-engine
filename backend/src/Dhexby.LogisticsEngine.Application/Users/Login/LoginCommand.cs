using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Users.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<Result<AuthResponse>>;
