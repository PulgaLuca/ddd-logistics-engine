using LogisticsEngine.Application.Hubs;
using LogisticsEngine.Domain.Hubs;
using LogisticsEngine.Domain.Hubs.Entities;
using LogisticsEngine.Domain.Hubs.Errors;
using LogisticsEngine.Domain.Hubs.Services;
using LogisticsEngine.SharedKernel;
using MediatR;

namespace LogisticsEngine.Application.Hubs.Algorithms;

public sealed class FindShortestHubPathQueryHandler(
    IHubRepository hubRepository,
    IHubConnectionRepository connectionRepository,
    IHubGraphDomainService graphService)
    : IRequestHandler<FindShortestHubPathQuery, Result<HubPathDto>>
{
    public Task<Result<HubPathDto>> Handle(FindShortestHubPathQuery query, CancellationToken cancellationToken)
    {
        if (query.OriginId == Guid.Empty || query.DestinationId == Guid.Empty)
        {
            return Task.FromResult(Result.Failure<HubPathDto>(HubErrors.InvalidId));
        }

        IReadOnlyList<Hub> hubs = hubRepository.GetAll();
        if (hubs.All(hub => hub.Id != query.OriginId))
        {
            return Task.FromResult(Result.Failure<HubPathDto>(HubErrors.NotFound(query.OriginId)));
        }

        if (hubs.All(hub => hub.Id != query.DestinationId))
        {
            return Task.FromResult(Result.Failure<HubPathDto>(HubErrors.NotFound(query.DestinationId)));
        }

        HubPath? path = graphService.FindShortestPath(
            hubs, connectionRepository.GetAll(), query.OriginId, query.DestinationId);
        if (path is null)
        {
            return Task.FromResult(Result.Failure<HubPathDto>(HubErrors.PathNotFound));
        }

        Dictionary<Guid, Hub> hubsById = hubs.ToDictionary(hub => hub.Id);
        HubPathDto result = new(path.HubIds.Select(id => HubDto.From(hubsById[id])).ToArray(), path.TotalDistance);
        return Task.FromResult(Result.Success(result));
    }
}
