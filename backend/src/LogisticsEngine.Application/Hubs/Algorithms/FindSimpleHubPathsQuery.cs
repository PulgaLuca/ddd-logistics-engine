using LogisticsEngine.Application.Hubs;
using LogisticsEngine.SharedKernel;
using MediatR;

namespace LogisticsEngine.Application.Hubs.Algorithms;

public sealed record FindSimpleHubPathsQuery(Guid OriginId, Guid DestinationId, int MaximumPaths)
    : IRequest<Result<IReadOnlyList<HubPathDto>>>;
