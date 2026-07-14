using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Routing;

/// <summary>Errori di dominio del calcolo percorsi / analisi di rete.</summary>
public static class RouteErrors
{
    public static readonly Error OriginNotInNetwork = Error.NotFound(
        "Route.Origin.NotInNetwork",
        "The origin hub is not part of the road network.");

    public static readonly Error DestinationNotInNetwork = Error.NotFound(
        "Route.Destination.NotInNetwork",
        "The destination hub is not part of the road network.");

    public static readonly Error NoRouteFound = Error.NotFound(
        "Route.NoRouteFound",
        "There is no route connecting the two hubs.");
}
