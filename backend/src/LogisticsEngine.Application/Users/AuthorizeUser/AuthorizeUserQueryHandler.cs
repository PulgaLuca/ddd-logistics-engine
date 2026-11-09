using MediatR;
using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.Application.Users.AuthorizeUser;

public sealed class AuthorizeUserQueryHandler(IUserAuthorizationService authorizationService)
    : IRequestHandler<AuthorizeUserQuery, Result<bool>>
{
    public Task<Result<bool>> Handle(AuthorizeUserQuery query, CancellationToken cancellationToken)
    {
        return Task.FromResult(authorizationService.Authorize(query.UserId, query.Permission));
    }
}
