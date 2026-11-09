using MediatR;
using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.Application.Users.AuthenticateUser;

public sealed class AuthenticateUserCommandHandler(IUserAuthenticationService authenticationService)
    : IRequestHandler<AuthenticateUserCommand, Result<UserDto>>
{
    public Task<Result<UserDto>> Handle(AuthenticateUserCommand command, CancellationToken cancellationToken)
    {
        var authenticated = authenticationService.Authenticate(command.UserName, command.Password);
        if (authenticated.IsFailure)
        {
            return Task.FromResult(Result.Failure<UserDto>(authenticated.Error));
        }

        return Task.FromResult(Result.Success(UserDto.From(authenticated.Value)));
    }
}
