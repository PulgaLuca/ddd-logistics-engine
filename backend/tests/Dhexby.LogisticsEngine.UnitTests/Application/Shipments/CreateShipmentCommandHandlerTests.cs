using Dhexby.LogisticsEngine.Application.Shipments;
using Dhexby.LogisticsEngine.Application.Shipments.CreateShipment;
using Dhexby.LogisticsEngine.Domain.Shipments;
using Dhexby.LogisticsEngine.UnitTests.Domain.Shipments;
using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.UnitTests.Application.Shipments;

public class CreateShipmentCommandHandlerTests
{
    private readonly InMemoryShipmentRepository _repository = new();
    private readonly CreateShipmentCommandHandler _handler;

    public CreateShipmentCommandHandlerTests()
    {
        _handler = new CreateShipmentCommandHandler(_repository, new ShipmentPricingService());
    }

    private static CreateShipmentCommand CreateValidCommand(string trackingNumber = ShipmentTestData.ValidTrackingNumber) =>
        new(
            trackingNumber,
            new AddressDto("Via Roma 1", "Milano", "20100", "IT"),
            new AddressDto("Via Garibaldi 2", "Torino", "10121", "IT"),
            WeightInKilograms: 10m,
            IsExpress: false);

    [Fact]
    public async Task Handle_ShouldPersistShipment_AndReturnItsId()
    {
        Result<Guid> result = await _handler.Handle(CreateValidCommand(), CancellationToken.None);

        Assert.True(result.IsSuccess);

        Shipment? saved = await _repository.GetByIdAsync(result.Value);
        Assert.NotNull(saved);
        Assert.Equal(ShipmentTestData.ValidTrackingNumber, saved.TrackingNumber.Value);
        Assert.Equal(17.00m, saved.Price.Amount); // prezzo calcolato dal domain service
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenTrackingNumberIsInvalid()
    {
        Result<Guid> result = await _handler.Handle(
            CreateValidCommand(trackingNumber: "not-valid"),
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ShipmentErrors.TrackingNumberInvalidFormat, result.Error);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenTrackingNumberIsAlreadyInUse()
    {
        await _handler.Handle(CreateValidCommand(), CancellationToken.None);

        Result<Guid> result = await _handler.Handle(CreateValidCommand(), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ShipmentErrors.TrackingNumberAlreadyInUse, result.Error);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenWeightIsInvalid()
    {
        CreateShipmentCommand command = CreateValidCommand() with { WeightInKilograms = 0m };

        Result<Guid> result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ShipmentErrors.WeightNotPositive, result.Error);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenOriginEqualsDestination()
    {
        CreateShipmentCommand command = CreateValidCommand() with
        {
            Destination = new AddressDto("Via Roma 1", "Milano", "20100", "IT"),
        };

        Result<Guid> result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ShipmentErrors.SameOriginAndDestination, result.Error);
    }
}
