using LogisticsEngine.Domain.Shipments.Entities;
using LogisticsEngine.Domain.Shipments.Enums;
using LogisticsEngine.Domain.Shipments.Errors;

namespace LogisticsEngine.UnitTests.Domain.Shipments;

public sealed class ShipmentTests
{
    [Fact]
    public void Create_requires_id_and_valid_tracking_number()
    {
        Shipment.Create(Guid.Empty, "ABCD1234").Error.ShouldBe(ShipmentErrors.InvalidId);
        Shipment.Create(Guid.NewGuid(), "bad").Error.ShouldBe(ShipmentErrors.InvalidTrackingNumber);
    }

    [Fact]
    public void Create_sets_identity_tracking_number_and_initial_status()
    {
        Guid id = Guid.NewGuid();
        Shipment shipment = Shipment.Create(id, " abcd1234 ").Value;
        shipment.Id.ShouldBe(id);
        shipment.TrackingNumber.Value.ShouldBe("ABCD1234");
        shipment.Status.ShouldBe(ShipmentStatus.Created);
    }

    [Fact]
    public void ChangeStatus_allows_only_valid_lifecycle_transitions()
    {
        Shipment shipment = Shipment.Create(Guid.NewGuid(), "ABCD1234").Value;

        shipment.ChangeStatus(ShipmentStatus.InTransit).IsSuccess.ShouldBeTrue();
        shipment.ChangeStatus(ShipmentStatus.Delivered).IsSuccess.ShouldBeTrue();
        shipment.ChangeStatus(ShipmentStatus.Cancelled).Error.ShouldBe(ShipmentErrors.InvalidTransition);
        shipment.Status.ShouldBe(ShipmentStatus.Delivered);
    }
}
