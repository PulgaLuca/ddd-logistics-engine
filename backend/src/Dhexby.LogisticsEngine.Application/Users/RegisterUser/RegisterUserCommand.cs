using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Users.RegisterUser;

public sealed record RegisterUserCommand(
    string Email,
    string Password,
    string Role) : IRequest<Result<AuthResponse>>;
