using Dhexby.LogisticsEngine.Domain.Zones;
using Dhexby.LogisticsEngine.Domain.Zones.Events;
using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.UnitTests.Domain.Zones;

public class ZoneTests
{
    private static Zone CreateRootZone() => Zone.Create(
        ZoneCode.Create("ZON-0001").Value,
        "Lombardia",
        parentZoneId: null,
        DateTime.UtcNow).Value;

    [Fact]
    public void Create_Root_ShouldHaveNoParent_AndRaiseEvent()
    {
        Zone zone = CreateRootZone();

        Assert.Null(zone.ParentZoneId);
        Assert.Empty(zone.Streets);
        Assert.Contains(zone.DomainEvents, domainEvent =>
            domainEvent is ZoneCreatedDomainEvent created && created.ParentZoneId is null);
    }

    [Fact]
    public void Create_Subzone_ShouldReferenceParent()
    {
        Guid parentId = Guid.NewGuid();

        Zone subzone = Zone.Create(
            ZoneCode.Create("ZON-0002").Value,
            "Milano",
            parentId,
            DateTime.UtcNow).Value;

        Assert.Equal(parentId, subzone.ParentZoneId);
    }

    [Fact]
    public void AddStreet_ShouldAppendStreet_AndRaiseEvent()
    {
        Zone zone = CreateRootZone();
        Street street = Street.Create("Via Roma", "20100").Value;

        Result result = zone.AddStreet(street);

        Assert.True(result.IsSuccess);
        Assert.Single(zone.Streets);
        Assert.Contains(zone.DomainEvents, domainEvent => domainEvent is StreetAddedToZoneDomainEvent);
    }

    [Fact]
    public void AddStreet_ShouldFail_WhenDuplicate()
    {
        Zone zone = CreateRootZone();
        zone.AddStreet(Street.Create("Via Roma", "20100").Value);

        Result result = zone.AddStreet(Street.Create("Via Roma", "20100").Value);

        Assert.True(result.IsFailure);
        Assert.Equal(ZoneErrors.DuplicateStreet, result.Error);
        Assert.Single(zone.Streets);
    }

    [Fact]
    public void Street_ShouldFail_WhenIncomplete()
    {
        Result<Street> result = Street.Create("Via Roma", "");

        Assert.True(result.IsFailure);
        Assert.Equal(ZoneErrors.StreetIncomplete, result.Error);
    }
}
