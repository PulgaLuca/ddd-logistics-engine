using Dhexby.LogisticsEngine.Application.Orders.PlaceOrder;
using FluentValidation.TestHelper;

namespace Dhexby.LogisticsEngine.UnitTests.Application.Orders;

public class PlaceOrderCommandValidatorTests
{
    private readonly PlaceOrderCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_ForValidCommand()
    {
        var command = new PlaceOrderCommand("ORD-ABC12345", "Mario Rossi", "Priority");

        _validator.TestValidate(command).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldFail_WhenPriorityInvalid()
    {
        var command = new PlaceOrderCommand("ORD-ABC12345", "Mario Rossi", "Nope");

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(command => command.Priority);
    }

    [Fact]
    public void Validate_ShouldFail_WhenOrderNumberMalformed()
    {
        var command = new PlaceOrderCommand("bad", "Mario Rossi", "Standard");

        _validator.TestValidate(command).ShouldHaveValidationErrorFor(command => command.OrderNumber);
    }
}
