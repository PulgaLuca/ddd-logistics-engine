using Dhexby.LogisticsEngine.Domain.Routing;

namespace Dhexby.LogisticsEngine.Infrastructure.Persistence;

/// <summary>Modello di persistenza della strada (solo primitivi).</summary>
internal sealed record RoadDocument(
    Guid Id,
    Guid FromHubId,
    Guid ToHubId,
    double DistanceInKilometers,
    double TrafficFactor,
    DateTime CreatedOnUtc)
{
    public static RoadDocument FromDomain(Road road) => new(
        road.Id,
        road.FromHubId,
        road.ToHubId,
        road.DistanceInKilometers,
        road.TrafficFactor,
        road.CreatedOnUtc);

    public Road ToDomain() => Road.Rehydrate(
        Id,
        FromHubId,
        ToHubId,
        DistanceInKilometers,
        TrafficFactor,
        CreatedOnUtc);
}
