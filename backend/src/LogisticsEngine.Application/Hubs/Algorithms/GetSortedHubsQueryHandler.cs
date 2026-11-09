using LogisticsEngine.Application.Hubs;
using LogisticsEngine.Domain.Hubs;
using LogisticsEngine.Domain.Hubs.Entities;
using LogisticsEngine.Domain.Hubs.Services;
using MediatR;

namespace LogisticsEngine.Application.Hubs.Algorithms;

public sealed class GetSortedHubsQueryHandler(IHubRepository repository, IHubSortingDomainService sortingService)
    : IRequestHandler<GetSortedHubsQuery, IReadOnlyList<HubDto>>
{
    public Task<IReadOnlyList<HubDto>> Handle(GetSortedHubsQuery query, CancellationToken cancellationToken)
    {
        IEnumerable<Hub> sorted = query.Property switch
        {
            HubSortProperty.Id => sortingService.SortById(repository.GetAll()),
            HubSortProperty.Name => sortingService.SortByName(repository.GetAll()),
            HubSortProperty.Zone => sortingService.SortByZone(repository.GetAll()),
            _ => []
        };

        return Task.FromResult<IReadOnlyList<HubDto>>(sorted.Select(HubDto.From).ToArray());
    }
}
