using LogisticsEngine.Application.Users;
using LogisticsEngine.SharedKernel;
using MediatR;

namespace LogisticsEngine.Application.Users.GetUserProfile;

public sealed record GetUserProfileQuery(Guid UserId) : IRequest<Result<UserDto>>;

public sealed class GetUserProfileQueryHandler(LogisticsEngine.Domain.Users.IUserRepository repository)
    : IRequestHandler<GetUserProfileQuery, Result<UserDto>>
{
    public Task<Result<UserDto>> Handle(GetUserProfileQuery query, CancellationToken cancellationToken)
    {
        var user = repository.GetById(query.UserId);
        return Task.FromResult(user is null
            ? Result.Failure<UserDto>(LogisticsEngine.Domain.Users.UserErrors.UserNotFound)
            : Result.Success(UserDto.From(user)));
    }
}
