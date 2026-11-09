using LogisticsEngine.Application.Hubs;
using LogisticsEngine.Domain.Hubs;
using LogisticsEngine.Domain.Hubs.Errors;
using LogisticsEngine.Domain.Hubs.Vo;
using LogisticsEngine.SharedKernel;
using MediatR;

namespace LogisticsEngine.Application.Hubs.Algorithms;

public sealed class ConnectHubsCommandHandler(IHubRepository hubRepository, IHubConnectionRepository connectionRepository)
    : IRequestHandler<ConnectHubsCommand, Result<HubConnectionDto>>
{
    public Task<Result<HubConnectionDto>> Handle(ConnectHubsCommand command, CancellationToken cancellationToken)
    {
        if (hubRepository.GetById(command.FirstHubId) is null)
        {
            return Task.FromResult(Result.Failure<HubConnectionDto>(HubErrors.NotFound(command.FirstHubId)));
        }

        if (hubRepository.GetById(command.SecondHubId) is null)
        {
            return Task.FromResult(Result.Failure<HubConnectionDto>(HubErrors.NotFound(command.SecondHubId)));
        }

        Result<HubConnection> created = HubConnection.Create(command.FirstHubId, command.SecondHubId, command.Distance);
        if (created.IsFailure)
        {
            return Task.FromResult(Result.Failure<HubConnectionDto>(created.Error));
        }

        bool exists = connectionRepository.GetAll().Any(connection =>
            connection.FirstHubId == created.Value.FirstHubId && connection.SecondHubId == created.Value.SecondHubId);
        if (exists)
        {
            return Task.FromResult(Result.Failure<HubConnectionDto>(HubErrors.DuplicateConnection));
        }

        connectionRepository.Add(created.Value);
        return Task.FromResult(Result.Success(new HubConnectionDto(
            created.Value.FirstHubId,
            created.Value.SecondHubId,
            created.Value.Distance)));
    }
}
