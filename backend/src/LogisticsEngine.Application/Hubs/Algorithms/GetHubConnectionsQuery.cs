using LogisticsEngine.Application.Hubs;
using MediatR;

namespace LogisticsEngine.Application.Hubs.Algorithms;

public sealed record GetHubConnectionsQuery : IRequest<IReadOnlyList<HubConnectionDto>>;
