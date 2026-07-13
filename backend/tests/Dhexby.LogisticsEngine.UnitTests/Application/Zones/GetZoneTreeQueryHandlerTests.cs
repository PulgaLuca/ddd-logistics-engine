using Dhexby.LogisticsEngine.Application.Zones.GetZoneTree;
using Dhexby.LogisticsEngine.Domain.Zones;
using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.UnitTests.Application.Zones;

public class GetZoneTreeQueryHandlerTests
{
    private readonly InMemoryZoneRepository _repository = new();
    private readonly GetZoneTreeQueryHandler _handler;

    public GetZoneTreeQueryHandlerTests()
    {
        _handler = new GetZoneTreeQueryHandler(_repository);
    }

    private async Task<Zone> AddZoneAsync(string code, string name, Guid? parentId)
    {
        Zone zone = Zone.Create(ZoneCode.Create(code).Value, name, parentId, DateTime.UtcNow).Value;
        await _repository.AddAsync(zone);
        return zone;
    }

    [Fact]
    public async Task Handle_ShouldBuildNestedTree_ZoneSubzoneStreets()
    {
        Zone region = await AddZoneAsync("ZON-0001", "Lombardia", null);
        Zone city = await AddZoneAsync("ZON-0002", "Milano", region.Id);
        await AddZoneAsync("ZON-0003", "Centro", city.Id);
        city.AddStreet(Street.Create("Via Roma", "20100").Value);
        await _repository.UpdateAsync(city);

        Result<ZoneTreeResponse> result = await _handler.Handle(
            new GetZoneTreeQuery(region.Id),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(region.Id, result.Value.ZoneId);

        ZoneTreeResponse milan = Assert.Single(result.Value.Subzones);
        Assert.Equal("Milano", milan.Name);
        Assert.Single(milan.Streets);
        Assert.Single(milan.Subzones); // Centro
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenRootMissing()
    {
        Guid missing = Guid.NewGuid();

        Result<ZoneTreeResponse> result = await _handler.Handle(new GetZoneTreeQuery(missing), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ZoneErrors.NotFound(missing), result.Error);
    }
}
