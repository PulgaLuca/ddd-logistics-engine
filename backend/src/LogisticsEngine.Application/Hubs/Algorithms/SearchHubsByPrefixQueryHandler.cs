using LogisticsEngine.Application.Hubs;
using LogisticsEngine.Domain.Hubs;
using LogisticsEngine.Domain.Hubs.Services;
using MediatR;

namespace LogisticsEngine.Application.Hubs.Algorithms;

public sealed class SearchHubsByPrefixQueryHandler(IHubRepository repository, IHubTrieDomainService trieService)
    : IRequestHandler<SearchHubsByPrefixQuery, IReadOnlyList<HubDto>>
{
    public Task<IReadOnlyList<HubDto>> Handle(SearchHubsByPrefixQuery query, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<HubDto>>(
            trieService.SearchByNamePrefix(repository.GetAll(), query.Prefix).Select(HubDto.From).ToArray());
}
