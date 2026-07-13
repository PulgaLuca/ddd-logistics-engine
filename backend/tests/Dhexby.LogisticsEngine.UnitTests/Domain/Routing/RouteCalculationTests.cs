using Dhexby.LogisticsEngine.Domain.Hubs;
using Dhexby.LogisticsEngine.Domain.Routing;
using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.UnitTests.Domain.Routing;

public class RouteCalculationTests
{
    // Rete a diamante:
    //   A -(short, congestionato)- B -(short)- D
    //   A -(long, scorrevole)----- C -(long)-- D
    private readonly Hub _a = RoutingTestData.CreateHub("HUB-000A", 45.00, 7.00);
    private readonly Hub _b = RoutingTestData.CreateHub("HUB-000B", 45.10, 7.05);
    private readonly Hub _c = RoutingTestData.CreateHub("HUB-000C", 45.00, 7.40);
    private readonly Hub _d = RoutingTestData.CreateHub("HUB-000D", 45.10, 7.45);

    private RoadNetwork BuildNetwork()
    {
        Road[] roads =
        [
            RoutingTestData.CreateRoad(_a.Id, _b.Id, distanceKm: 10, trafficFactor: 5.0),
            RoutingTestData.CreateRoad(_b.Id, _d.Id, distanceKm: 10, trafficFactor: 5.0),
            RoutingTestData.CreateRoad(_a.Id, _c.Id, distanceKm: 15, trafficFactor: 1.0),
            RoutingTestData.CreateRoad(_c.Id, _d.Id, distanceKm: 15, trafficFactor: 1.0),
        ];

        return RoadNetwork.Build([_a, _b, _c, _d], roads);
    }

    [Fact]
    public void Dijkstra_ShouldPickShortestByDistance_IgnoringTraffic()
    {
        var dijkstra = new DijkstraRouteCalculationService();

        Result<RoutePath> result = dijkstra.FindRoute(BuildNetwork(), _a.Id, _d.Id);

        Assert.True(result.IsSuccess);
        // Distanza: A-B-D = 20 km < A-C-D = 30 km.
        Assert.Equal([_a.Id, _b.Id, _d.Id], result.Value.HubIds);
        Assert.Equal(20, result.Value.TotalCost);
    }

    [Fact]
    public void AStar_ShouldPickTrafficAwareRoute()
    {
        var planner = new SmartRoutePlanner();

        Result<RoutePath> result = planner.FindRoute(BuildNetwork(), _a.Id, _d.Id);

        Assert.True(result.IsSuccess);
        // Costo smart: A-B-D = (10*5)+(10*5)=100 ; A-C-D = 15+15=30 -> sceglie C.
        Assert.Equal([_a.Id, _c.Id, _d.Id], result.Value.HubIds);
        Assert.Equal(30, result.Value.TotalCost);
    }

    [Fact]
    public void FindRoute_ShouldFail_WhenNoRoute()
    {
        Hub isolated = RoutingTestData.CreateHub("HUB-000E", 40, 5);
        RoadNetwork network = RoadNetwork.Build([_a, _b, isolated], [RoutingTestData.CreateRoad(_a.Id, _b.Id, 10)]);

        Result<RoutePath> result = new DijkstraRouteCalculationService().FindRoute(network, _a.Id, isolated.Id);

        Assert.True(result.IsFailure);
        Assert.Equal(RouteErrors.NoRouteFound, result.Error);
    }

    [Fact]
    public void FindRoute_ShouldFail_WhenOriginUnknown()
    {
        RoadNetwork network = RoadNetwork.Build([_a, _b], [RoutingTestData.CreateRoad(_a.Id, _b.Id, 10)]);

        Result<RoutePath> result = new DijkstraRouteCalculationService().FindRoute(network, Guid.NewGuid(), _b.Id);

        Assert.True(result.IsFailure);
        Assert.Equal(RouteErrors.OriginNotInNetwork, result.Error);
    }
}
