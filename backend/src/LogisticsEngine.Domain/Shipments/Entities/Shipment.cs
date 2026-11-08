using LogisticsEngine.Domain.Shipments.Errors;
using LogisticsEngine.Domain.Shipments.Enums;
using LogisticsEngine.Domain.Shipments.ValueObjects;
using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.Domain.Shipments.Entities;

/// <summary>Entità Shipment con identità stabile e stato modificabile solo tramite comportamenti di dominio.</summary>
public sealed class Shipment : Entity
{
    private Shipment(Guid id, TrackingNumber trackingNumber) : base(id)
    {
        TrackingNumber = trackingNumber;
        Status = ShipmentStatus.Created;
    }

    public TrackingNumber TrackingNumber { get; }
    public ShipmentStatus Status { get; private set; }

    public static Result<Shipment> Create(Guid id, string? trackingNumber)
    {
        if (id == Guid.Empty)
        {
            return Result.Failure<Shipment>(ShipmentErrors.InvalidId);
        }

        Result<TrackingNumber> number = TrackingNumber.Create(trackingNumber);
        return number.IsFailure ? Result.Failure<Shipment>(number.Error) : Result.Success(new Shipment(id, number.Value));
    }

    public Result ChangeStatus(ShipmentStatus status)
    {
        if (!CanTransitionTo(status))
        {
            return Result.Failure(ShipmentErrors.InvalidTransition);
        }

        Status = status;
        return Result.Success();
    }

    private bool CanTransitionTo(ShipmentStatus status) => Status switch
    {
        ShipmentStatus.Created => status is ShipmentStatus.InTransit or ShipmentStatus.Cancelled,
        ShipmentStatus.InTransit => status is ShipmentStatus.Delivered or ShipmentStatus.Cancelled,
        _ => false
    };
}
