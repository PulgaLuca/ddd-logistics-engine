using Dhexby.LogisticsEngine.Domain.Shipments;
using Dhexby.LogisticsEngine.Domain.Shipments.Events;
using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.UnitTests.Domain.Shipments;

public class ShipmentTests
{
    [Fact]
    public void Create_ShouldStartPending_AndRaiseCreatedEvent()
    {
        Shipment shipment = ShipmentTestData.CreateShipment();

        Assert.Equal(ShipmentStatus.Pending, shipment.Status);
        Assert.Null(shipment.DeliveredOnUtc);
        Assert.Contains(shipment.DomainEvents, domainEvent =>
            domainEvent is ShipmentCreatedDomainEvent created && created.ShipmentId == shipment.Id);
    }

    [Fact]
    public void Create_ShouldFail_WhenOriginEqualsDestination()
    {
        Address address = ShipmentTestData.CreateOrigin();

        Result<Shipment> result = Shipment.Create(
            ShipmentTestData.CreateTrackingNumber(),
            address,
            address,
            ShipmentTestData.CreateWeight(),
            ShipmentTestData.CreatePrice(),
            isExpress: false,
            DateTime.UtcNow);

        Assert.True(result.IsFailure);
        Assert.Equal(ShipmentErrors.SameOriginAndDestination, result.Error);
    }

    [Fact]
    public void Dispatch_ShouldMoveToInTransit_AndRaiseEvent()
    {
        Shipment shipment = ShipmentTestData.CreateShipment();

        Result result = shipment.Dispatch();

        Assert.True(result.IsSuccess);
        Assert.Equal(ShipmentStatus.InTransit, shipment.Status);
        Assert.Contains(shipment.DomainEvents, domainEvent => domainEvent is ShipmentDispatchedDomainEvent);
    }

    [Fact]
    public void Dispatch_ShouldFail_WhenNotPending()
    {
        Shipment shipment = ShipmentTestData.CreateShipment();
        shipment.Dispatch();

        Result result = shipment.Dispatch();

        Assert.True(result.IsFailure);
        Assert.Equal(ShipmentErrors.NotPending, result.Error);
    }

    [Fact]
    public void MarkAsDelivered_ShouldSucceed_WhenInTransit()
    {
        Shipment shipment = ShipmentTestData.CreateShipment();
        shipment.Dispatch();
        DateTime deliveredOn = DateTime.UtcNow;

        Result result = shipment.MarkAsDelivered(deliveredOn);

        Assert.True(result.IsSuccess);
        Assert.Equal(ShipmentStatus.Delivered, shipment.Status);
        Assert.Equal(deliveredOn, shipment.DeliveredOnUtc);
        Assert.Contains(shipment.DomainEvents, domainEvent => domainEvent is ShipmentDeliveredDomainEvent);
    }

    [Fact]
    public void MarkAsDelivered_ShouldFail_WhenNotInTransit()
    {
        Shipment shipment = ShipmentTestData.CreateShipment();

        Result result = shipment.MarkAsDelivered(DateTime.UtcNow);

        Assert.True(result.IsFailure);
        Assert.Equal(ShipmentErrors.NotInTransit, result.Error);
    }

    [Fact]
    public void Cancel_ShouldSucceed_WhenPending()
    {
        Shipment shipment = ShipmentTestData.CreateShipment();

        Result result = shipment.Cancel();

        Assert.True(result.IsSuccess);
        Assert.Equal(ShipmentStatus.Cancelled, shipment.Status);
        Assert.Contains(shipment.DomainEvents, domainEvent => domainEvent is ShipmentCancelledDomainEvent);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Cancel_ShouldFail_WhenAlreadyDispatched(bool alsoDelivered)
    {
        Shipment shipment = ShipmentTestData.CreateShipment();
        shipment.Dispatch();
        if (alsoDelivered)
        {
            shipment.MarkAsDelivered(DateTime.UtcNow);
        }

        Result result = shipment.Cancel();

        Assert.True(result.IsFailure);
        Assert.Equal(ShipmentErrors.CannotCancel, result.Error);
    }

    [Fact]
    public void Rehydrate_ShouldRestoreState_WithoutRaisingEvents()
    {
        Shipment original = ShipmentTestData.CreateShipment();

        Shipment rehydrated = Shipment.Rehydrate(
            original.Id,
            original.TrackingNumber,
            original.Origin,
            original.Destination,
            original.Weight,
            original.Price,
            original.IsExpress,
            ShipmentStatus.InTransit,
            original.CreatedOnUtc,
            deliveredOnUtc: null);

        Assert.Equal(original.Id, rehydrated.Id);
        Assert.Equal(ShipmentStatus.InTransit, rehydrated.Status);
        Assert.Empty(rehydrated.DomainEvents);
    }
}
