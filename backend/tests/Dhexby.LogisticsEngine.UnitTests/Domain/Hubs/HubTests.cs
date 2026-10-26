using Dhexby.LogisticsEngine.Domain.Hubs;
using Dhexby.LogisticsEngine.Domain.Hubs.Events;
using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.UnitTests.Domain.Hubs;

public class HubTests
{
    private static Hub CreateHub() => Hub.Create(
        HubCode.Create("HUB-0001").Value,
        //"Milano Central Hub",
        GeoCoordinate.Create(45.4642, 9.1900).Value,
        DateTime.UtcNow).Value;

    [Fact]
    public void Create_ShouldStartActive_AndRaiseRegisteredEvent()
    {
        Hub hub = CreateHub();

        Assert.Equal(HubStatus.Active, hub.Status);
        Assert.Contains(hub.DomainEvents, domainEvent => domainEvent is HubRegisteredDomainEvent);
    }

    //[Theory]
    //[InlineData(null)]
    //[InlineData("   ")]
    //public void Create_ShouldFail_WhenNameMissing(string? name)
    //{
    //    Result<Hub> result = Hub.Create(
    //        HubCode.Create("HUB-0001").Value,
    //        name,
    //        GeoCoordinate.Create(0, 0).Value,
    //        DateTime.UtcNow);

    //    Assert.True(result.IsFailure);
    //    Assert.Equal(HubErrors.NameRequired, result.Error);
    //}

    [Fact]
    public void Deactivate_ShouldSucceed_WhenActive_ThenFailIfRepeated()
    {
        Hub hub = CreateHub();

        Result first = hub.Deactivate();
        Result second = hub.Deactivate();

        Assert.True(first.IsSuccess);
        Assert.Equal(HubStatus.Inactive, hub.Status);
        Assert.True(second.IsFailure);
        Assert.Equal(HubErrors.AlreadyInactive, second.Error);
    }

    [Fact]
    public void Activate_ShouldFail_WhenAlreadyActive()
    {
        Hub hub = CreateHub();

        Result result = hub.Activate();

        Assert.True(result.IsFailure);
        Assert.Equal(HubErrors.AlreadyActive, result.Error);
    }
}
