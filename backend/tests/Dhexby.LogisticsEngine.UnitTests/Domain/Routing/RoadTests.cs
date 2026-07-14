using Dhexby.LogisticsEngine.Domain.Routing;
using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.UnitTests.Domain.Routing;

public class RoadTests
{
    [Fact]
    public void Create_ShouldSucceed_AndRaiseEvent()
    {
        Guid from = Guid.NewGuid();
        Guid to = Guid.NewGuid();

        Result<Road> result = Road.Create(from, to, 10, 1.5, DateTime.UtcNow);

        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Value.DomainEvents);
    }

    [Fact]
    public void Create_ShouldFail_WhenSameHubs()
    {
        Guid hub = Guid.NewGuid();

        Result<Road> result = Road.Create(hub, hub, 10, 1, DateTime.UtcNow);

        Assert.True(result.IsFailure);
        Assert.Equal(RoadErrors.SameHubs, result.Error);
    }

    [Fact]
    public void Create_ShouldFail_WhenDistanceNotPositive()
    {
        Result<Road> result = Road.Create(Guid.NewGuid(), Guid.NewGuid(), 0, 1, DateTime.UtcNow);

        Assert.True(result.IsFailure);
        Assert.Equal(RoadErrors.DistanceNotPositive, result.Error);
    }

    [Theory]
    [InlineData(0.5)]
    [InlineData(11)]
    public void Create_ShouldFail_WhenTrafficOutOfRange(double traffic)
    {
        Result<Road> result = Road.Create(Guid.NewGuid(), Guid.NewGuid(), 10, traffic, DateTime.UtcNow);

        Assert.True(result.IsFailure);
        Assert.Equal(RoadErrors.TrafficFactorOutOfRange, result.Error);
    }
}
