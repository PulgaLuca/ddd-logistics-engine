using Dhexby.LogisticsEngine.Domain.Hubs;
using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.UnitTests.Domain.Hubs;

public class GeoCoordinateTests
{
    [Fact]
    public void Create_ShouldSucceed_WhenWithinRanges()
    {
        Result<GeoCoordinate> result = GeoCoordinate.Create(45.07, 7.69);

        Assert.True(result.IsSuccess);
        Assert.Equal(45.07, result.Value.Latitude);
    }

    // verifica che la creazione di una coordinata geografica fallisca
    // quando la latitudine è al di fuori dell'intervallo consentito [-90, 90].
    [Theory]
    [InlineData(-91, 0)]
    [InlineData(91, 0)]
    public void Create_ShouldFail_WhenLatitudeOutOfRange(double latitude, double longitude)
    {
        Result<GeoCoordinate> result = GeoCoordinate.Create(latitude, longitude);

        // verifica che il risultato rappresenti un fallimento
        Assert.True(result.IsFailure);

        // verifica che l'errore restituito sia quello relativo alla latitudine fuori intervallo
        Assert.Equal(HubErrors.LatitudeOutOfRange, result.Error);
    }

    [Theory]
    [InlineData(0, -181)]
    [InlineData(0, 181)]
    public void Create_ShouldFail_WhenLongitudeOutOfRange(double latitude, double longitude)
    {
        Result<GeoCoordinate> result = GeoCoordinate.Create(latitude, longitude);

        Assert.True(result.IsFailure);
        Assert.Equal(HubErrors.LongitudeOutOfRange, result.Error);
    }

    [Fact]
    public void DistanceInKilometersTo_ShouldBeZero_ForSamePoint()
    {
        GeoCoordinate point = GeoCoordinate.Create(45.07, 7.69).Value;

        Assert.Equal(0, point.DistanceInKilometersTo(point), precision: 6);
    }

    [Fact]
    public void DistanceInKilometersTo_ShouldApproximateKnownDistance()
    {
        // Milano (~45.4642, 9.1900) -> Torino (~45.0703, 7.6869): circa 125 km.
        GeoCoordinate milan = GeoCoordinate.Create(45.4642, 9.1900).Value;
        GeoCoordinate turin = GeoCoordinate.Create(45.0703, 7.6869).Value;

        double distance = milan.DistanceInKilometersTo(turin);

        Assert.InRange(distance, 115, 135);
    }
}
