using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Routing.GetHubReachability;

/// <summary>Query: verifica se un hub è raggiungibile da un altro (BFS/DFS).</summary>
public sealed record GetHubReachabilityQuery(Guid OriginHubId, Guid DestinationHubId)
    : IRequest<Result<ReachabilityResponse>>;
