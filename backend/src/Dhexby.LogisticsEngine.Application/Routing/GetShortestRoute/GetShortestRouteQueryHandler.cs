using Dhexby.LogisticsEngine.Domain.Routing;
using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Routing.GetShortestRoute;

/// <summary>Use case: costruisce la rete e calcola il percorso più corto (Dijkstra).</summary>
public sealed class GetShortestRouteQueryHandler : IRequestHandler<GetShortestRouteQuery, Result<RouteResponse>>
{
    private readonly IRoadNetworkFactory _networkFactory;
    private readonly DijkstraRouteCalculationService _calculator;

    public GetShortestRouteQueryHandler(
        IRoadNetworkFactory networkFactory,
        DijkstraRouteCalculationService calculator)
    {
        _networkFactory = networkFactory;
        _calculator = calculator;
    }

    public async Task<Result<RouteResponse>> Handle(
        GetShortestRouteQuery request,
        CancellationToken cancellationToken)
    {
        RoadNetworkContext context = await _networkFactory.BuildAsync(cancellationToken);

        Result<RoutePath> routeResult = _calculator.FindRoute(
            context.Network,
            request.OriginHubId,
            request.DestinationHubId);

        if (routeResult.IsFailure)
        {
            return Result.Failure<RouteResponse>(routeResult.Error);
        }

        return RouteResponse.From(_calculator.Strategy, routeResult.Value, context.Hubs);
    }
}
