using Dhexby.LogisticsEngine.Domain.Routing.Events;
using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Routing;

/// <summary>
/// Aggregate root della strada: un arco (bidirezionale) della rete logistica
/// che collega due hub. Porta la distanza fisica e un fattore di traffico
/// (moltiplicatore &gt;= 1) usato dal routing "smart" (A*).
/// </summary>
public sealed class Road : Entity
{
    public const double MinTrafficFactor = 1.0;
    public const double MaxTrafficFactor = 10.0;

    private Road(
        Guid id,
        Guid fromHubId,
        Guid toHubId,
        double distanceInKilometers,
        double trafficFactor,
        DateTime createdOnUtc)
        : base(id)
    {
        FromHubId = fromHubId;
        ToHubId = toHubId;
        DistanceInKilometers = distanceInKilometers;
        TrafficFactor = trafficFactor;
        CreatedOnUtc = createdOnUtc;
    }

    public Guid FromHubId { get; private set; }

    public Guid ToHubId { get; private set; }

    public double DistanceInKilometers { get; private set; }

    public double TrafficFactor { get; private set; }

    public DateTime CreatedOnUtc { get; private set; }

    public static Result<Road> Create(
        Guid fromHubId,
        Guid toHubId,
        double distanceInKilometers,
        double trafficFactor,
        DateTime utcNow)
    {
        if (fromHubId == Guid.Empty || toHubId == Guid.Empty)
        {
            return Result.Failure<Road>(RoadErrors.HubRequired);
        }

        if (fromHubId == toHubId)
        {
            return Result.Failure<Road>(RoadErrors.SameHubs);
        }

        if (distanceInKilometers <= 0)
        {
            return Result.Failure<Road>(RoadErrors.DistanceNotPositive);
        }

        if (trafficFactor is < MinTrafficFactor or > MaxTrafficFactor)
        {
            return Result.Failure<Road>(RoadErrors.TrafficFactorOutOfRange);
        }

        var road = new Road(Guid.NewGuid(), fromHubId, toHubId, distanceInKilometers, trafficFactor, utcNow);
        road.RaiseDomainEvent(new RoadAddedDomainEvent(road.Id, fromHubId, toHubId));

        return road;
    }

    public static Road Rehydrate(
        Guid id,
        Guid fromHubId,
        Guid toHubId,
        double distanceInKilometers,
        double trafficFactor,
        DateTime createdOnUtc) =>
        new(id, fromHubId, toHubId, distanceInKilometers, trafficFactor, createdOnUtc);
}
