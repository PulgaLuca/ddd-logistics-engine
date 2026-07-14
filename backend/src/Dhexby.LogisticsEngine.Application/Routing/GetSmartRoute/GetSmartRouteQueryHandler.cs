using Dhexby.LogisticsEngine.Domain.Routing;
using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Routing.GetSmartRoute;

/// <summary>Use case: costruisce la rete e calcola il percorso smart (A*, traffico).</summary>
public sealed class GetSmartRouteQueryHandler : IRequestHandler<GetSmartRouteQuery, Result<RouteResponse>>
{
    private readonly IRoadNetworkFactory _networkFactory;
    private readonly SmartRoutePlanner _planner;

    public GetSmartRouteQueryHandler(IRoadNetworkFactory networkFactory, SmartRoutePlanner planner)
    {
        _networkFactory = networkFactory;
        _planner = planner;
    }

    public async Task<Result<RouteResponse>> Handle(GetSmartRouteQuery request, CancellationToken cancellationToken)
    {
        RoadNetworkContext context = await _networkFactory.BuildAsync(cancellationToken);

        Result<RoutePath> routeResult = _planner.FindRoute(
            context.Network,
            request.OriginHubId,
            request.DestinationHubId);

        if (routeResult.IsFailure)
        {
            return Result.Failure<RouteResponse>(routeResult.Error);
        }

        return RouteResponse.From(_planner.Strategy, routeResult.Value, context.Hubs);
    }
}
