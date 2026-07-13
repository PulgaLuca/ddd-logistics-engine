using Dhexby.LogisticsEngine.Application.Routing;
using Dhexby.LogisticsEngine.Application.Routing.AddRoad;
using Dhexby.LogisticsEngine.Application.Routing.GetHubReachability;
using Dhexby.LogisticsEngine.Application.Routing.GetShortestRoute;
using Dhexby.LogisticsEngine.Application.Routing.GetSmartRoute;
using Dhexby.LogisticsEngine.Domain.Hubs;
using Dhexby.LogisticsEngine.Domain.Routing;
using Dhexby.LogisticsEngine.UnitTests.Application.Hubs;
using Dhexby.LogisticsEngine.UnitTests.Domain.Routing;
using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.UnitTests.Application.Routing;

public class RoutingHandlersTests
{
    private readonly InMemoryHubRepository _hubRepository = new();
    private readonly InMemoryRoadRepository _roadRepository = new();
    private readonly RoadNetworkFactory _networkFactory;

    public RoutingHandlersTests()
    {
        _networkFactory = new RoadNetworkFactory(_hubRepository, _roadRepository);
    }

    private async Task<(Hub A, Hub B, Hub C)> SeedLineNetworkAsync()
    {
        Hub a = RoutingTestData.CreateHub("HUB-000A", 45.00, 7.00);
        Hub b = RoutingTestData.CreateHub("HUB-000B", 45.10, 7.05);
        Hub c = RoutingTestData.CreateHub("HUB-000C", 45.20, 7.10);
        await _hubRepository.AddAsync(a);
        await _hubRepository.AddAsync(b);
        await _hubRepository.AddAsync(c);
        await _roadRepository.AddAsync(RoutingTestData.CreateRoad(a.Id, b.Id, 10));
        await _roadRepository.AddAsync(RoutingTestData.CreateRoad(b.Id, c.Id, 10));
        return (a, b, c);
    }

    [Fact]
    public async Task AddRoad_ShouldPersist_WhenHubsExist()
    {
        Hub a = RoutingTestData.CreateHub("HUB-000A", 45, 7);
        Hub b = RoutingTestData.CreateHub("HUB-000B", 46, 8);
        await _hubRepository.AddAsync(a);
        await _hubRepository.AddAsync(b);
        var handler = new AddRoadCommandHandler(_roadRepository, _hubRepository);

        Result<Guid> result = await handler.Handle(new AddRoadCommand(a.Id, b.Id, 12, 1.2), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(await _roadRepository.ExistsBetweenAsync(a.Id, b.Id));
    }

    [Fact]
    public async Task AddRoad_ShouldFail_WhenHubMissing()
    {
        Hub a = RoutingTestData.CreateHub("HUB-000A", 45, 7);
        await _hubRepository.AddAsync(a);
        Guid missing = Guid.NewGuid();
        var handler = new AddRoadCommandHandler(_roadRepository, _hubRepository);

        Result<Guid> result = await handler.Handle(new AddRoadCommand(a.Id, missing, 12, 1), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(HubErrors.NotFound(missing), result.Error);
    }

    [Fact]
    public async Task ShortestRoute_ShouldReturnPath_WithDijkstraStrategy()
    {
        (Hub a, Hub b, Hub c) = await SeedLineNetworkAsync();
        var handler = new GetShortestRouteQueryHandler(_networkFactory, new DijkstraRouteCalculationService());

        Result<RouteResponse> result = await handler.Handle(
            new GetShortestRouteQuery(a.Id, c.Id),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Dijkstra", result.Value.Strategy);
        Assert.Equal([a.Id, b.Id, c.Id], result.Value.Nodes.Select(node => node.HubId));
        Assert.Equal(2, result.Value.HopCount);
    }

    [Fact]
    public async Task SmartRoute_ShouldReturnPath_WithAStarStrategy()
    {
        (Hub a, _, Hub c) = await SeedLineNetworkAsync();
        var handler = new GetSmartRouteQueryHandler(_networkFactory, new SmartRoutePlanner());

        Result<RouteResponse> result = await handler.Handle(
            new GetSmartRouteQuery(a.Id, c.Id),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("AStar", result.Value.Strategy);
        Assert.True(result.Value.Found);
    }

    [Fact]
    public async Task Reachability_ShouldReportReachable_WithBothTraversals()
    {
        (Hub a, _, Hub c) = await SeedLineNetworkAsync();
        var handler = new GetHubReachabilityQueryHandler(_networkFactory, new NetworkAnalysisService());

        Result<ReachabilityResponse> result = await handler.Handle(
            new GetHubReachabilityQuery(a.Id, c.Id),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value.CanReach);
        Assert.Equal(3, result.Value.Path.Count);
        Assert.NotEmpty(result.Value.BreadthFirstOrder);
        Assert.NotEmpty(result.Value.DepthFirstOrder);
    }
}
