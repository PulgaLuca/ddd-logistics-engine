using Dhexby.LogisticsEngine.Domain.Shipments;
using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.UnitTests.Domain.Shipments;

public class AddressTests
{
    [Fact]
    public void Create_ShouldSucceed_AndTrimFields()
    {
        Result<Address> result = Address.Create(" Via Roma 1 ", " Milano ", " 20100 ", " IT ");

        Assert.True(result.IsSuccess);
        Assert.Equal("Via Roma 1", result.Value.Street);
        Assert.Equal("Milano", result.Value.City);
    }

    [Theory]
    [InlineData(null, "Milano", "20100", "IT")]
    [InlineData("Via Roma 1", "", "20100", "IT")]
    [InlineData("Via Roma 1", "Milano", "  ", "IT")]
    [InlineData("Via Roma 1", "Milano", "20100", null)]
    public void Create_ShouldFail_WhenAnyFieldIsMissing(string? street, string? city, string? postalCode, string? country)
    {
        Result<Address> result = Address.Create(street, city, postalCode, country);

        Assert.True(result.IsFailure);
        Assert.Equal(ShipmentErrors.AddressIncomplete, result.Error);
    }

    [Fact]
    public void Equality_ShouldBeStructural()
    {
        Address first = ShipmentTestData.CreateOrigin();
        Address second = ShipmentTestData.CreateOrigin();

        Assert.Equal(first, second);
        Assert.NotEqual(first, ShipmentTestData.CreateDestination());
    }
}
