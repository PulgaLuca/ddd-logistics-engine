using Dhexby.LogisticsEngine.Domain.Shipments.Events;
using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Shipments;

/// <summary>
/// Aggregate root della spedizione. Tutte le transizioni di stato passano
/// dai metodi di dominio, che applicano le business rules e ritornano Result.
/// </summary>
public sealed class Shipment : Entity
{
    private Shipment(
        Guid id,
        TrackingNumber trackingNumber,
        Address origin,
        Address destination,
        Weight weight,
        Money price,
        bool isExpress,
        ShipmentStatus status,
        DateTime createdOnUtc)
        : base(id)
    {
        TrackingNumber = trackingNumber;
        Origin = origin;
        Destination = destination;
        Weight = weight;
        Price = price;
        IsExpress = isExpress;
        Status = status;
        CreatedOnUtc = createdOnUtc;
    }

    public TrackingNumber TrackingNumber { get; private set; }

    public Address Origin { get; private set; }

    public Address Destination { get; private set; }

    public Weight Weight { get; private set; }

    public Money Price { get; private set; }

    public bool IsExpress { get; private set; }

    public ShipmentStatus Status { get; private set; }

    public DateTime CreatedOnUtc { get; private set; }

    public DateTime? DeliveredOnUtc { get; private set; }

    /// <summary>
    /// Factory method: unico punto di creazione valido di una spedizione.
    /// Business rule: origine e destinazione devono essere diverse.
    /// </summary>
    public static Result<Shipment> Create(
        TrackingNumber trackingNumber,
        Address origin,
        Address destination,
        Weight weight,
        Money price,
        bool isExpress,
        DateTime utcNow)
    {
        if (origin.Equals(destination))
        {
            return Result.Failure<Shipment>(ShipmentErrors.SameOriginAndDestination);
        }

        var shipment = new Shipment(
            Guid.NewGuid(),
            trackingNumber,
            origin,
            destination,
            weight,
            price,
            isExpress,
            ShipmentStatus.Pending,
            utcNow);

        shipment.RaiseDomainEvent(new ShipmentCreatedDomainEvent(shipment.Id));

        return shipment;
    }

    /// <summary>Business rule: si può spedire solo una spedizione Pending.</summary>
    public Result Dispatch()
    {
        if (Status != ShipmentStatus.Pending)
        {
            return Result.Failure(ShipmentErrors.NotPending);
        }

        Status = ShipmentStatus.InTransit;
        RaiseDomainEvent(new ShipmentDispatchedDomainEvent(Id));

        return Result.Success();
    }

    /// <summary>Business rule: si può consegnare solo una spedizione InTransit.</summary>
    public Result MarkAsDelivered(DateTime utcNow)
    {
        if (Status != ShipmentStatus.InTransit)
        {
            return Result.Failure(ShipmentErrors.NotInTransit);
        }

        Status = ShipmentStatus.Delivered;
        DeliveredOnUtc = utcNow;
        RaiseDomainEvent(new ShipmentDeliveredDomainEvent(Id));

        return Result.Success();
    }

    /// <summary>Business rule: si può annullare solo una spedizione Pending.</summary>
    public Result Cancel()
    {
        if (Status != ShipmentStatus.Pending)
        {
            return Result.Failure(ShipmentErrors.CannotCancel);
        }

        Status = ShipmentStatus.Cancelled;
        RaiseDomainEvent(new ShipmentCancelledDomainEvent(Id));

        return Result.Success();
    }

    /// <summary>
    /// Ricostruisce l'aggregato dallo store di persistenza senza rieseguire
    /// le regole di creazione né sollevare eventi. Da usare solo nei repository.
    /// </summary>
    public static Shipment Rehydrate(
        Guid id,
        TrackingNumber trackingNumber,
        Address origin,
        Address destination,
        Weight weight,
        Money price,
        bool isExpress,
        ShipmentStatus status,
        DateTime createdOnUtc,
        DateTime? deliveredOnUtc)
    {
        return new Shipment(id, trackingNumber, origin, destination, weight, price, isExpress, status, createdOnUtc)
        {
            DeliveredOnUtc = deliveredOnUtc,
        };
    }
}
