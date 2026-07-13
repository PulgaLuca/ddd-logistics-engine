using Dhexby.LogisticsEngine.Application.Shipments.DispatchShipment;
using FluentValidation.TestHelper;

namespace Dhexby.LogisticsEngine.UnitTests.Application.Shipments;

public class DispatchShipmentCommandValidatorTests
{
    private readonly DispatchShipmentCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenShipmentIdIsProvided()
    {
        TestValidationResult<DispatchShipmentCommand> result =
            _validator.TestValidate(new DispatchShipmentCommand(Guid.NewGuid()));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldFail_WhenShipmentIdIsEmpty()
    {
        TestValidationResult<DispatchShipmentCommand> result =
            _validator.TestValidate(new DispatchShipmentCommand(Guid.Empty));

        result.ShouldHaveValidationErrorFor(command => command.ShipmentId);
    }
}
