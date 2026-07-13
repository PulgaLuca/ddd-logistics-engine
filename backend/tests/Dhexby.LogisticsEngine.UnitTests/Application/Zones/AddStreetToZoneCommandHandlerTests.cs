using Dhexby.LogisticsEngine.Application.Zones.AddStreetToZone;
using Dhexby.LogisticsEngine.Domain.Zones;
using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.UnitTests.Application.Zones;

public class AddStreetToZoneCommandHandlerTests
{
    private readonly InMemoryZoneRepository _repository = new();
    private readonly AddStreetToZoneCommandHandler _handler;

    public AddStreetToZoneCommandHandlerTests()
    {
        _handler = new AddStreetToZoneCommandHandler(_repository);
    }

    private async Task<Zone> SeedZoneAsync()
    {
        Zone zone = Zone.Create(ZoneCode.Create("ZON-0001").Value, "Lombardia", null, DateTime.UtcNow).Value;
        await _repository.AddAsync(zone);
        return zone;
    }

    [Fact]
    public async Task Handle_ShouldAddStreet_AndPersist()
    {
        Zone zone = await SeedZoneAsync();

        Result result = await _handler.Handle(
            new AddStreetToZoneCommand(zone.Id, "Via Roma", "20100"),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(zone.Streets);
        Assert.Equal(1, _repository.UpdateCallCount);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenZoneMissing()
    {
        Guid missingZone = Guid.NewGuid();

        Result result = await _handler.Handle(
            new AddStreetToZoneCommand(missingZone, "Via Roma", "20100"),
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ZoneErrors.NotFound(missingZone), result.Error);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenStreetDuplicated()
    {
        Zone zone = await SeedZoneAsync();
        await _handler.Handle(new AddStreetToZoneCommand(zone.Id, "Via Roma", "20100"), CancellationToken.None);

        Result result = await _handler.Handle(
            new AddStreetToZoneCommand(zone.Id, "Via Roma", "20100"),
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ZoneErrors.DuplicateStreet, result.Error);
    }
}
