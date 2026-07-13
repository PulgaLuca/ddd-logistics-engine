using Dhexby.LogisticsEngine.Domain.Hubs;
using Dhexby.LogisticsEngine.Domain.Routing;

namespace Dhexby.LogisticsEngine.UnitTests.Domain.Routing;

/// <summary>Helper per costruire hub e strade nei test del routing.</summary>
internal static class RoutingTestData
{
    public static Hub CreateHub(string code, double latitude, double longitude) => Hub.Create(
        HubCode.Create(code).Value,
        $"Hub {code}",
        GeoCoordinate.Create(latitude, longitude).Value,
        DateTime.UtcNow).Value;

    public static Road CreateRoad(Guid fromHubId, Guid toHubId, double distanceKm, double trafficFactor = 1.0) =>
        Road.Create(fromHubId, toHubId, distanceKm, trafficFactor, DateTime.UtcNow).Value;
}
