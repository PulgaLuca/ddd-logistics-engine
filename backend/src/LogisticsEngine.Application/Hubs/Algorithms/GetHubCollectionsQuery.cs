using LogisticsEngine.Application.Hubs;
using MediatR;

namespace LogisticsEngine.Application.Hubs.Algorithms;

public sealed record GetHubCollectionsQuery : IRequest<HubCollectionsDto>;

public sealed record HubCollectionsDto(
    IReadOnlyList<HubDto> QueueOrder,
    IReadOnlyList<HubDto> StackOrder,
    IReadOnlyDictionary<Guid, HubDto> IdHashMap,
    IReadOnlySet<string> Zones);
