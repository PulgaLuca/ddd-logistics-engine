using Dhexby.LogisticsEngine.Domain.Shipments;
using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.UnitTests.Domain.Shipments;

public class ShipmentPricingServiceTests
{
    private readonly ShipmentPricingService _pricingService = new();

    [Theory]
    [InlineData(10, 17.00)]  // 5 + 10 * 1.20
    [InlineData(1, 6.20)]    // 5 + 1 * 1.20
    public void CalculatePrice_ShouldApplyBasePlusPerKilogramRate(decimal kilograms, decimal expectedAmount)
    {
        Weight weight = Weight.Create(kilograms).Value;

        Result<Money> result = _pricingService.CalculatePrice(weight, isExpress: false);

        Assert.True(result.IsSuccess);
        Assert.Equal(expectedAmount, result.Value.Amount);
        Assert.Equal(ShipmentPricingService.Currency, result.Value.Currency);
    }

    [Fact]
    public void CalculatePrice_ShouldApplyExpressMultiplier()
    {
        Weight weight = Weight.Create(10m).Value;

        Result<Money> result = _pricingService.CalculatePrice(weight, isExpress: true);

        Assert.True(result.IsSuccess);
        Assert.Equal(25.50m, result.Value.Amount); // (5 + 12) * 1.5
    }
}
