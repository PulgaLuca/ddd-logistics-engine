using Dhexby.LogisticsEngine.Domain.Orders;
using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.UnitTests.Domain.Orders;

public class OrderNumberTests
{
    [Fact]
    public void Create_ShouldNormalize_WhenValid()
    {
        Result<OrderNumber> result = OrderNumber.Create("  ord-abc12345 ");

        Assert.True(result.IsSuccess);
        Assert.Equal("ORD-ABC12345", result.Value.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_ShouldFail_WhenEmpty(string? value)
    {
        Result<OrderNumber> result = OrderNumber.Create(value);

        Assert.True(result.IsFailure);
        Assert.Equal(OrderErrors.OrderNumberEmpty, result.Error);
    }

    [Theory]
    [InlineData("ABC12345")]
    [InlineData("ORD-123")]
    public void Create_ShouldFail_WhenFormatInvalid(string value)
    {
        Result<OrderNumber> result = OrderNumber.Create(value);

        Assert.True(result.IsFailure);
        Assert.Equal(OrderErrors.OrderNumberInvalidFormat, result.Error);
    }

    [Fact]
    public void CustomerName_ShouldFail_WhenTooLong()
    {
        Result<CustomerName> result = CustomerName.Create(new string('a', CustomerName.MaxLength + 1));

        Assert.True(result.IsFailure);
        Assert.Equal(OrderErrors.CustomerNameTooLong, result.Error);
    }
}
