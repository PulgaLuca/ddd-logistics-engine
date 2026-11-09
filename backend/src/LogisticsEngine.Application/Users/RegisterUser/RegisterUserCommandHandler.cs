using LogisticsEngine.Domain.Users;
using LogisticsEngine.Domain.Users.Entities;
using LogisticsEngine.Domain.Users.Enums;
using LogisticsEngine.Domain.Users.ValueObjects;
using LogisticsEngine.SharedKernel;
using MediatR;

namespace LogisticsEngine.Application.Users.RegisterUser;

public sealed class RegisterUserCommandHandler(IUserRepository repository, IPasswordHasher passwordHasher)
    : IRequestHandler<RegisterUserCommand, Result<UserDto>>
{
    public Task<Result<UserDto>> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        Result<UserName> userName = UserName.Create(command.UserName);
        if (userName.IsFailure)
        {
            return Task.FromResult(Result.Failure<UserDto>(userName.Error));
        }

        if (repository.GetByUserName(userName.Value) is not null)
        {
            return Task.FromResult(Result.Failure<UserDto>(UserErrors.DuplicateUserName));
        }

        // Il testo della password viene hashato prima di creare l'utente e non entra mai nel repository.
        string hash = passwordHasher.Hash(command.Password!);
        Result<User> user = User.Create(Guid.NewGuid(), userName.Value.Value, hash, UserRole.Viewer);
        if (user.IsFailure)
        {
            return Task.FromResult(Result.Failure<UserDto>(user.Error));
        }

        repository.Add(user.Value);
        return Task.FromResult(Result.Success(UserDto.From(user.Value)));
    }
}
