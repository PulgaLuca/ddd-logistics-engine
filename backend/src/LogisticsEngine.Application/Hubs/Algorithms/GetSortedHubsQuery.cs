using LogisticsEngine.Application.Hubs;
using MediatR;

namespace LogisticsEngine.Application.Hubs.Algorithms;

public enum HubSortProperty
{
    Id,
    Name,
    Zone
}

public sealed record GetSortedHubsQuery(HubSortProperty Property) : IRequest<IReadOnlyList<HubDto>>;
