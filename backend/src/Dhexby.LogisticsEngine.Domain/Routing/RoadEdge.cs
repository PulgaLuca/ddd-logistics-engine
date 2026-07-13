namespace Dhexby.LogisticsEngine.Domain.Routing;

/// <summary>
/// Arco uscente nel grafo della rete. <see cref="SmartCost"/> pondera la
/// distanza per il traffico ed è il costo usato dal routing smart (A*).
/// </summary>
public sealed record RoadEdge(Guid ToHubId, double DistanceInKilometers, double TrafficFactor)
{
    public double SmartCost => DistanceInKilometers * TrafficFactor;
}
