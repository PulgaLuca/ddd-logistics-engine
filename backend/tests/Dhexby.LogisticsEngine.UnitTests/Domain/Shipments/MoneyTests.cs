using Dhexby.LogisticsEngine.Domain.Shipments;
using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.UnitTests.Domain.Shipments;

public class MoneyTests
{
    [Fact]
    public void Create_ShouldRoundToTwoDecimals_AndNormalizeCurrency()
    {
        Result<Money> result = Money.Create(10.005m, "eur");

        Assert.True(result.IsSuccess);
        Assert.Equal(10.00m, result.Value.Amount);
        Assert.Equal("EUR", result.Value.Currency);
    }

    [Fact]
    public void Create_ShouldFail_WhenAmountIsNegative()
    {
        Result<Money> result = Money.Create(-1m, "EUR");

        Assert.True(result.IsFailure);
        Assert.Equal(ShipmentErrors.MoneyNegative, result.Error);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("EURO")]
    public void Create_ShouldFail_WhenCurrencyIsInvalid(string? currency)
    {
        Result<Money> result = Money.Create(10m, currency);

        Assert.True(result.IsFailure);
        Assert.Equal(ShipmentErrors.MoneyInvalidCurrency, result.Error);
    }
}
