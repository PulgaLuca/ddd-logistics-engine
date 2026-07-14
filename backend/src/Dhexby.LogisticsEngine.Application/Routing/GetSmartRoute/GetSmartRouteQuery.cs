using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Routing.GetSmartRoute;

/// <summary>Query: percorso "smart" traffico-consapevole (A*) tra due hub.</summary>
public sealed record GetSmartRouteQuery(Guid OriginHubId, Guid DestinationHubId)
    : IRequest<Result<RouteResponse>>;
