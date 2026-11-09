using LogisticsEngine.Application.Hubs;
using LogisticsEngine.Domain.Hubs;
using LogisticsEngine.Domain.Hubs.Services;
using MediatR;

namespace LogisticsEngine.Application.Hubs.Algorithms;

public sealed class GetHubTreeOrderQueryHandler(IHubRepository repository, IHubTreeDomainService treeService)
    : IRequestHandler<GetHubTreeOrderQuery, IReadOnlyList<HubDto>>
{
    public Task<IReadOnlyList<HubDto>> Handle(GetHubTreeOrderQuery query, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<HubDto>>(treeService.TraverseInOrder(repository.GetAll()).Select(HubDto.From).ToArray());
}
