using LogisticsEngine.Application.Hubs;
using LogisticsEngine.Domain.Hubs;
using LogisticsEngine.Domain.Hubs.Entities;
using LogisticsEngine.Domain.Hubs.Services;
using MediatR;

namespace LogisticsEngine.Application.Hubs.Algorithms;

public sealed class GetHubCollectionsQueryHandler(
    IHubRepository repository,
    IHubQueueDomainService queueService,
    IHubStackDomainService stackService,
    IHubHashMapDomainService hashMapService,
    IHubSetDomainService setService)
    : IRequestHandler<GetHubCollectionsQuery, HubCollectionsDto>
{
    public Task<HubCollectionsDto> Handle(GetHubCollectionsQuery query, CancellationToken cancellationToken)
    {
        IReadOnlyList<Hub> hubs = repository.GetAll();
        IReadOnlyDictionary<Guid, HubDto> index = hashMapService.IndexById(hubs)
            .ToDictionary(entry => entry.Key, entry => HubDto.From(entry.Value));
        var result = new HubCollectionsDto(
            queueService.ProcessInFifoOrder(hubs).Select(HubDto.From).ToArray(),
            stackService.PopInLifoOrder(hubs).Select(HubDto.From).ToArray(),
            index,
            setService.DistinctZones(hubs));
        return Task.FromResult(result);
    }
}
