using Dhexby.LogisticsEngine.Domain.Hubs;
using Dhexby.LogisticsEngine.Domain.Routing;
using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.UnitTests.Domain.Routing;

public class NetworkAnalysisServiceTests
{
    private readonly NetworkAnalysisService _service = new();

    private readonly Hub _a = RoutingTestData.CreateHub("HUB-000A", 45.00, 7.00);
    private readonly Hub _b = RoutingTestData.CreateHub("HUB-000B", 45.10, 7.05);
    private readonly Hub _c = RoutingTestData.CreateHub("HUB-000C", 45.00, 7.40);
    private readonly Hub _isolated = RoutingTestData.CreateHub("HUB-000Z", 40.00, 5.00);

    private RoadNetwork BuildNetwork() => RoadNetwork.Build(
        [_a, _b, _c, _isolated],
        [RoutingTestData.CreateRoad(_a.Id, _b.Id, 10), RoutingTestData.CreateRoad(_b.Id, _c.Id, 10)]);

    [Fact]
    public void CheckReachability_ShouldReturnPath_WhenReachable()
    {
        Result<ReachabilityResult> result = _service.CheckReachability(BuildNetwork(), _a.Id, _c.Id);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value.CanReach);
        Assert.Equal([_a.Id, _b.Id, _c.Id], result.Value.Path);
        Assert.NotEmpty(result.Value.BreadthFirstOrder);
        Assert.NotEmpty(result.Value.DepthFirstOrder);
    }

    [Fact]
    public void CheckReachability_ShouldReportUnreachable_ForIsolatedHub()
    {
        Result<ReachabilityResult> result = _service.CheckReachability(BuildNetwork(), _a.Id, _isolated.Id);

        Assert.True(result.IsSuccess);
        Assert.False(result.Value.CanReach);
        Assert.Empty(result.Value.Path);
    }

    [Fact]
    public void CheckReachability_ShouldFail_WhenDestinationUnknown()
    {
        Result<ReachabilityResult> result = _service.CheckReachability(BuildNetwork(), _a.Id, Guid.NewGuid());

        Assert.True(result.IsFailure);
        Assert.Equal(RouteErrors.DestinationNotInNetwork, result.Error);
    }
}
