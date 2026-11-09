using LogisticsEngine.Application.Hubs;
using MediatR;

namespace LogisticsEngine.Application.Hubs.Algorithms;

public sealed record SearchHubsByPrefixQuery(string? Prefix) : IRequest<IReadOnlyList<HubDto>>;
