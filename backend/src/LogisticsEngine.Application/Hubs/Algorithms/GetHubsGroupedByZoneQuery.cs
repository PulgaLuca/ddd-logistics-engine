using LogisticsEngine.Application.Hubs;
using MediatR;

namespace LogisticsEngine.Application.Hubs.Algorithms;

public sealed record GetHubsGroupedByZoneQuery : IRequest<IReadOnlyList<HubGroupDto>>;
