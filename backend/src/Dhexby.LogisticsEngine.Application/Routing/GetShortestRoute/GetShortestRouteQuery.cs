using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Routing.GetShortestRoute;

/// <summary>Query: percorso più corto per distanza (Dijkstra) tra due hub.</summary>
public sealed record GetShortestRouteQuery(Guid OriginHubId, Guid DestinationHubId)
    : IRequest<Result<RouteResponse>>;
