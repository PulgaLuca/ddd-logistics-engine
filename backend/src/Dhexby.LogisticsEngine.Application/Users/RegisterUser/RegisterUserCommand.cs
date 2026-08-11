using Dhexby.LogisticsEngine.Application.Abstractions.Messaging;

namespace Dhexby.LogisticsEngine.Application.Users.RegisterUser;

public sealed record RegisterUserCommand(
    string Email,
    string Password,
    string Role) : ICommand<AuthResponse>;
