using Dhexby.LogisticsEngine.Domain.Shipments;
using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.UnitTests.Domain.Shipments;

public class WeightTests
{
    [Theory]
    [InlineData(0.1)]
    [InlineData(10)]
    [InlineData(1000)]
    public void Create_ShouldSucceed_WhenWithinBounds(decimal kilograms)
    {
        Result<Weight> result = Weight.Create(kilograms);

        Assert.True(result.IsSuccess);
        Assert.Equal(kilograms, result.Value.Kilograms);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_ShouldFail_WhenNotPositive(decimal kilograms)
    {
        Result<Weight> result = Weight.Create(kilograms);

        Assert.True(result.IsFailure);
        Assert.Equal(ShipmentErrors.WeightNotPositive, result.Error);
    }

    [Fact]
    public void Create_ShouldFail_WhenAboveMaximum()
    {
        Result<Weight> result = Weight.Create(Weight.MaxKilograms + 1);

        Assert.True(result.IsFailure);
        Assert.Equal(ShipmentErrors.WeightTooHeavy, result.Error);
    }
}
