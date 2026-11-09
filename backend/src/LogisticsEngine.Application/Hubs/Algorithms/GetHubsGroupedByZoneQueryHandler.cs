using LogisticsEngine.Application.Hubs;
using LogisticsEngine.Domain.Hubs;
using LogisticsEngine.Domain.Hubs.Services;
using MediatR;

namespace LogisticsEngine.Application.Hubs.Algorithms;

public sealed class GetHubsGroupedByZoneQueryHandler(IHubRepository repository, IHubGroupingDomainService groupingService)
    : IRequestHandler<GetHubsGroupedByZoneQuery, IReadOnlyList<HubGroupDto>>
{
    public Task<IReadOnlyList<HubGroupDto>> Handle(GetHubsGroupedByZoneQuery query, CancellationToken cancellationToken)
    {
        IReadOnlyList<HubGroupDto> groups = groupingService.GroupByZone(repository.GetAll())
            .Select(group => new HubGroupDto(group[0].Zone.Code, group.Select(HubDto.From).ToArray()))
            .ToArray();
        return Task.FromResult(groups);
    }
}
