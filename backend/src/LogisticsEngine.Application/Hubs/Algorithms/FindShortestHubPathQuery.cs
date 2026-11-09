using LogisticsEngine.Application.Hubs;
using LogisticsEngine.SharedKernel;
using MediatR;

namespace LogisticsEngine.Application.Hubs.Algorithms;

public sealed record FindShortestHubPathQuery(Guid OriginId, Guid DestinationId)
    : IRequest<Result<HubPathDto>>;
