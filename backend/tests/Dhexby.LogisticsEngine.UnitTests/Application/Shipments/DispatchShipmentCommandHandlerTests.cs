using Dhexby.LogisticsEngine.Application.Shipments.DispatchShipment;
using Dhexby.LogisticsEngine.Domain.Shipments;
using Dhexby.LogisticsEngine.UnitTests.Domain.Shipments;
using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.UnitTests.Application.Shipments;

public class DispatchShipmentCommandHandlerTests
{
    private readonly InMemoryShipmentRepository _repository = new();
    private readonly DispatchShipmentCommandHandler _handler;

    public DispatchShipmentCommandHandlerTests()
    {
        _handler = new DispatchShipmentCommandHandler(_repository);
    }

    [Fact]
    public async Task Handle_ShouldDispatchShipment_AndPersistChange()
    {
        Shipment shipment = ShipmentTestData.CreateShipment();
        await _repository.AddAsync(shipment);

        Result result = await _handler.Handle(new DispatchShipmentCommand(shipment.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(ShipmentStatus.InTransit, shipment.Status);
        Assert.Equal(1, _repository.UpdateCallCount);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenShipmentDoesNotExist()
    {
        Guid unknownId = Guid.NewGuid();

        Result result = await _handler.Handle(new DispatchShipmentCommand(unknownId), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ShipmentErrors.NotFound(unknownId), result.Error);
    }

    [Fact]
    public async Task Handle_ShouldFail_AndNotPersist_WhenShipmentIsNotPending()
    {
        Shipment shipment = ShipmentTestData.CreateShipment();
        shipment.Dispatch();
        await _repository.AddAsync(shipment);

        Result result = await _handler.Handle(new DispatchShipmentCommand(shipment.Id), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ShipmentErrors.NotPending, result.Error);
        Assert.Equal(0, _repository.UpdateCallCount);
    }
}
