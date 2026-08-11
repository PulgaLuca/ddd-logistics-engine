using Dhexby.LogisticsEngine.Application.Abstractions.Messaging;

namespace Dhexby.LogisticsEngine.Application.Users.Login;

public sealed record LoginCommand(string Email, string Password) : ICommand<AuthResponse>;
