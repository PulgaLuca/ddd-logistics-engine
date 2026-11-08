using LogisticsEngine.Domain.Shipments.Errors;
using LogisticsEngine.Domain.Shipments.ValueObjects;

namespace LogisticsEngine.UnitTests.Domain.Shipments;

public sealed class TrackingNumberTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("ABC1234")]
    [InlineData("1234567890123456789012345")]
    [InlineData("ABC-12345")]
    public void Create_rejects_invalid_numbers(string? value) => TrackingNumber.Create(value).Error.ShouldBe(ShipmentErrors.InvalidTrackingNumber);

    [Theory]
    [InlineData("ABCD1234")]
    [InlineData("123456789012345678901234")]
    public void Create_trims_and_normalizes(string value)
    {
        TrackingNumber result = TrackingNumber.Create($" {value.ToLowerInvariant()} ").Value;
        result.Value.ShouldBe(value.ToUpperInvariant());
        result.ToString().ShouldBe(result.Value);
    }

    [Fact]
    public void Equality_uses_normalized_value()
    {
        TrackingNumber first = TrackingNumber.Create("abcd1234").Value;
        TrackingNumber second = TrackingNumber.Create("ABCD1234").Value;
        first.Equals(second).ShouldBeTrue();
        first.GetHashCode().ShouldBe(second.GetHashCode());
    }
}
