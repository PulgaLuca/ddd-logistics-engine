using Dhexby.LogisticsEngine.Domain.Shipments;
using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.UnitTests.Domain.Shipments;

public class TrackingNumberTests
{
    [Fact]
    public void Create_ShouldSucceed_WhenFormatIsValid()
    {
        Result<TrackingNumber> result = TrackingNumber.Create("TRK-ABC12345");

        Assert.True(result.IsSuccess);
        Assert.Equal("TRK-ABC12345", result.Value.Value);
    }

    [Fact]
    public void Create_ShouldNormalizeToUpperCase_AndTrim()
    {
        Result<TrackingNumber> result = TrackingNumber.Create("  trk-abc12345  ");

        Assert.True(result.IsSuccess);
        Assert.Equal("TRK-ABC12345", result.Value.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ShouldFail_WhenValueIsMissing(string? value)
    {
        Result<TrackingNumber> result = TrackingNumber.Create(value);

        Assert.True(result.IsFailure);
        Assert.Equal(ShipmentErrors.TrackingNumberEmpty, result.Error);
    }

    [Theory]
    [InlineData("ABC12345")]
    [InlineData("TRK-ABC")]
    [InlineData("TRK-ABC123456")]
    [InlineData("TRK-ABC-1234")]
    public void Create_ShouldFail_WhenFormatIsInvalid(string value)
    {
        Result<TrackingNumber> result = TrackingNumber.Create(value);

        Assert.True(result.IsFailure);
        Assert.Equal(ShipmentErrors.TrackingNumberInvalidFormat, result.Error);
    }

    [Fact]
    public void Equality_ShouldBeStructural()
    {
        TrackingNumber first = ShipmentTestData.CreateTrackingNumber();
        TrackingNumber second = ShipmentTestData.CreateTrackingNumber();

        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }
}
