using LogisticsEngine.Application.Users;
using LogisticsEngine.SharedKernel;
using MediatR;

namespace LogisticsEngine.Application.Users.UpdateUserProfile;

public sealed record UpdateUserProfileCommand(Guid UserId, string? FirstName, string? LastName, string? ProfessionalRole)
    : IRequest<Result<UserDto>>;

public sealed class UpdateUserProfileCommandHandler(LogisticsEngine.Domain.Users.IUserRepository repository)
    : IRequestHandler<UpdateUserProfileCommand, Result<UserDto>>
{
    public Task<Result<UserDto>> Handle(UpdateUserProfileCommand command, CancellationToken cancellationToken)
    {
        var user = repository.GetById(command.UserId);
        if (user is null)
            return Task.FromResult(Result.Failure<UserDto>(LogisticsEngine.Domain.Users.UserErrors.UserNotFound));

        var updateResult = user.UpdateProfile(command.FirstName, command.LastName, command.ProfessionalRole);
        if (updateResult.IsFailure)
            return Task.FromResult(Result.Failure<UserDto>(updateResult.Error));

        repository.Update(user);
        return Task.FromResult(Result.Success(UserDto.From(user)));
    }
}
