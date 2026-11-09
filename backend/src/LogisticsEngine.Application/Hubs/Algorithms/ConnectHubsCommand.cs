using LogisticsEngine.Application.Hubs;
using LogisticsEngine.SharedKernel;
using MediatR;

namespace LogisticsEngine.Application.Hubs.Algorithms;

public sealed record ConnectHubsCommand(Guid FirstHubId, Guid SecondHubId, decimal Distance)
    : IRequest<Result<HubConnectionDto>>;
