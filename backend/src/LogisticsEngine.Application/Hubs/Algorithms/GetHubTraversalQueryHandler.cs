using LogisticsEngine.Application.Hubs;
using LogisticsEngine.Domain.Hubs;
using LogisticsEngine.Domain.Hubs.Entities;
using LogisticsEngine.Domain.Hubs.Errors;
using LogisticsEngine.Domain.Hubs.Services;
using LogisticsEngine.SharedKernel;
using MediatR;

namespace LogisticsEngine.Application.Hubs.Algorithms;

public sealed class GetHubTraversalQueryHandler(IHubRepository hubRepository, IHubConnectionRepository connectionRepository, IHubGraphDomainService graphService)
    : IRequestHandler<GetHubTraversalQuery, Result<IReadOnlyList<HubDto>>>
{
    public Task<Result<IReadOnlyList<HubDto>>> Handle(GetHubTraversalQuery query, CancellationToken cancellationToken)
    {
        if (query.OriginId == Guid.Empty)
        {
            return Task.FromResult(Result.Failure<IReadOnlyList<HubDto>>(HubErrors.InvalidId));
        }

        IReadOnlyList<Hub> hubs = hubRepository.GetAll();
        if (hubs.All(hub => hub.Id != query.OriginId))
        {
            return Task.FromResult(Result.Failure<IReadOnlyList<HubDto>>(HubErrors.NotFound(query.OriginId)));
        }

        IReadOnlyList<Guid> traversal = query.Traversal switch
        {
            HubGraphTraversal.BreadthFirst => graphService.BreadthFirstSearch(hubs, connectionRepository.GetAll(), query.OriginId),
            HubGraphTraversal.DepthFirst => graphService.DepthFirstSearch(hubs, connectionRepository.GetAll(), query.OriginId),
            _ => []
        };
        Dictionary<Guid, Hub> hubsById = hubs.ToDictionary(hub => hub.Id);
        IReadOnlyList<HubDto> result = traversal.Select(id => HubDto.From(hubsById[id])).ToArray();
        return Task.FromResult(Result.Success(result));
    }
}
