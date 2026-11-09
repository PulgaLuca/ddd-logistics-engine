using LogisticsEngine.Application.Hubs;
using LogisticsEngine.Domain.Hubs;
using LogisticsEngine.Domain.Hubs.Entities;
using LogisticsEngine.Domain.Hubs.Errors;
using LogisticsEngine.Domain.Hubs.Services;
using LogisticsEngine.Domain.Hubs.Vo;
using LogisticsEngine.SharedKernel;
using MediatR;

namespace LogisticsEngine.Application.Hubs.Algorithms;

public sealed class FindSimpleHubPathsQueryHandler(
    IHubRepository hubRepository,
    IHubConnectionRepository connectionRepository,
    IHubBacktrackingDomainService backtrackingService)
    : IRequestHandler<FindSimpleHubPathsQuery, Result<IReadOnlyList<HubPathDto>>>
{
    public Task<Result<IReadOnlyList<HubPathDto>>> Handle(FindSimpleHubPathsQuery query, CancellationToken cancellationToken)
    {
        if (query.OriginId == Guid.Empty || query.DestinationId == Guid.Empty)
        {
            return Task.FromResult(Result.Failure<IReadOnlyList<HubPathDto>>(HubErrors.InvalidId));
        }

        if (query.MaximumPaths <= 0)
        {
            return Task.FromResult(Result.Failure<IReadOnlyList<HubPathDto>>(HubErrors.InvalidPathLimit));
        }

        IReadOnlyList<Hub> hubs = hubRepository.GetAll();
        Dictionary<Guid, Hub> hubsById = hubs.ToDictionary(hub => hub.Id);
        if (!hubsById.ContainsKey(query.OriginId))
        {
            return Task.FromResult(Result.Failure<IReadOnlyList<HubPathDto>>(HubErrors.NotFound(query.OriginId)));
        }

        if (!hubsById.ContainsKey(query.DestinationId))
        {
            return Task.FromResult(Result.Failure<IReadOnlyList<HubPathDto>>(HubErrors.NotFound(query.DestinationId)));
        }

        IReadOnlyList<HubConnection> connections = connectionRepository.GetAll();
        IReadOnlyList<HubPathDto> paths = backtrackingService.FindSimplePaths(
                hubs, connections, query.OriginId, query.DestinationId, query.MaximumPaths)
            .Select(path => new HubPathDto(
                path.Select(id => HubDto.From(hubsById[id])).ToArray(),
                GetDistance(path, connections)))
            .ToArray();
        return Task.FromResult(Result.Success(paths));
    }

    private static decimal GetDistance(IReadOnlyList<Guid> path, IReadOnlyList<HubConnection> connections) =>
        path.Zip(path.Skip(1), (firstHubId, secondHubId) => FindConnectionDistance(firstHubId, secondHubId, connections)).Sum();

    private static decimal FindConnectionDistance(Guid firstHubId, Guid secondHubId, IReadOnlyList<HubConnection> connections) =>
        connections.First(connection =>
            (connection.FirstHubId == firstHubId && connection.SecondHubId == secondHubId)
            || (connection.FirstHubId == secondHubId && connection.SecondHubId == firstHubId)).Distance;
}
