using Dhexby.LogisticsEngine.Application.Shipments;
using Dhexby.LogisticsEngine.Application.Shipments.CreateShipment;
using FluentValidation.TestHelper;

namespace Dhexby.LogisticsEngine.UnitTests.Application.Shipments;

public class CreateShipmentCommandValidatorTests
{
    private readonly CreateShipmentCommandValidator _validator = new();

    private static CreateShipmentCommand ValidCommand() => new(
        "TRK-ABC12345",
        new AddressDto("Via Roma 1", "Milano", "20100", "IT"),
        new AddressDto("Via Garibaldi 2", "Torino", "10121", "IT"),
        WeightInKilograms: 10m,
        IsExpress: false);

    [Fact]
    public void Validate_ShouldPass_ForValidCommand()
    {
        TestValidationResult<CreateShipmentCommand> result = _validator.TestValidate(ValidCommand());

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-valid")]
    [InlineData("TRK-ABC")]
    public void Validate_ShouldFail_WhenTrackingNumberIsMalformed(string trackingNumber)
    {
        CreateShipmentCommand command = ValidCommand() with { TrackingNumber = trackingNumber };

        TestValidationResult<CreateShipmentCommand> result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(command => command.TrackingNumber);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    [InlineData(1001)]
    public void Validate_ShouldFail_WhenWeightIsOutOfRange(decimal weight)
    {
        CreateShipmentCommand command = ValidCommand() with { WeightInKilograms = weight };

        TestValidationResult<CreateShipmentCommand> result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(command => command.WeightInKilograms);
    }

    [Fact]
    public void Validate_ShouldFail_WhenOriginFieldsAreMissing()
    {
        CreateShipmentCommand command = ValidCommand() with
        {
            Origin = new AddressDto("", "", "", ""),
        };

        TestValidationResult<CreateShipmentCommand> result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("Origin.Street");
        result.ShouldHaveValidationErrorFor("Origin.City");
        result.ShouldHaveValidationErrorFor("Origin.PostalCode");
        result.ShouldHaveValidationErrorFor("Origin.Country");
    }
}
