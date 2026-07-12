using Dhexby.LogisticsEngine.Application.Shipments;
using Dhexby.LogisticsEngine.Application.Shipments.GetShipmentById;
using Dhexby.LogisticsEngine.Domain.Shipments;
using Dhexby.LogisticsEngine.UnitTests.Domain.Shipments;
using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.UnitTests.Application.Shipments;

public class GetShipmentByIdQueryHandlerTests
{
    private readonly InMemoryShipmentRepository _repository = new();
    private readonly GetShipmentByIdQueryHandler _handler;

    public GetShipmentByIdQueryHandlerTests()
    {
        _handler = new GetShipmentByIdQueryHandler(_repository);
    }

    [Fact]
    public async Task Handle_ShouldReturnResponse_WhenShipmentExists()
    {
        Shipment shipment = ShipmentTestData.CreateShipment();
        await _repository.AddAsync(shipment);

        Result<ShipmentResponse> result = await _handler.Handle(
            new GetShipmentByIdQuery(shipment.Id),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(shipment.Id, result.Value.Id);
        Assert.Equal(shipment.TrackingNumber.Value, result.Value.TrackingNumber);
        Assert.Equal(nameof(ShipmentStatus.Pending), result.Value.Status);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenShipmentDoesNotExist()
    {
        Guid unknownId = Guid.NewGuid();

        Result<ShipmentResponse> result = await _handler.Handle(
            new GetShipmentByIdQuery(unknownId),
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ShipmentErrors.NotFound(unknownId), result.Error);
    }
}
