using Dhexby.LogisticsEngine.Domain.Orders.Events;
using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Orders;

/// <summary>
/// Aggregate root dell'ordine. La coppia <see cref="AssignToHub"/> /
/// <see cref="Unassign"/> è simmetrica: rende naturale, in futuro, una struttura
/// a Stack per undo/redo dell'assegnazione a un hub.
/// </summary>
public sealed class Order : Entity
{
    private Order(
        Guid id,
        OrderNumber orderNumber,
        CustomerName customerName,
        OrderPriority priority,
        OrderStatus status,
        Guid? assignedHubId,
        DateTime placedOnUtc)
        : base(id)
    {
        OrderNumber = orderNumber;
        CustomerName = customerName;
        Priority = priority;
        Status = status;
        AssignedHubId = assignedHubId;
        PlacedOnUtc = placedOnUtc;
    }

    public OrderNumber OrderNumber { get; private set; }

    public CustomerName CustomerName { get; private set; }

    public OrderPriority Priority { get; private set; }

    public OrderStatus Status { get; private set; }

    public Guid? AssignedHubId { get; private set; }

    public DateTime PlacedOnUtc { get; private set; }

    /// <summary>Factory: unico punto di creazione. L'ordine nasce Pending.</summary>
    public static Result<Order> Create(
        OrderNumber orderNumber,
        CustomerName customerName,
        OrderPriority priority,
        DateTime utcNow)
    {
        var order = new Order(
            Guid.NewGuid(),
            orderNumber,
            customerName,
            priority,
            OrderStatus.Pending,
            assignedHubId: null,
            utcNow);

        order.RaiseDomainEvent(new OrderPlacedDomainEvent(order.Id));

        return order;
    }

    /// <summary>Business rule: solo un ordine Pending può essere assegnato a un hub.</summary>
    public Result AssignToHub(Guid hubId)
    {
        if (hubId == Guid.Empty)
        {
            return Result.Failure(OrderErrors.HubIdRequired);
        }

        if (Status != OrderStatus.Pending)
        {
            return Result.Failure(OrderErrors.NotPending);
        }

        Status = OrderStatus.Assigned;
        AssignedHubId = hubId;
        RaiseDomainEvent(new OrderAssignedDomainEvent(Id, hubId));

        return Result.Success();
    }

    /// <summary>
    /// Business rule: annulla l'assegnazione riportando l'ordine a Pending.
    /// È l'operazione inversa di <see cref="AssignToHub"/> (undo).
    /// </summary>
    public Result Unassign()
    {
        if (Status != OrderStatus.Assigned || AssignedHubId is null)
        {
            return Result.Failure(OrderErrors.NotAssigned);
        }

        Guid previousHubId = AssignedHubId.Value;
        Status = OrderStatus.Pending;
        AssignedHubId = null;
        RaiseDomainEvent(new OrderUnassignedDomainEvent(Id, previousHubId));

        return Result.Success();
    }

    /// <summary>Business rule: solo un ordine Assigned può essere completato.</summary>
    public Result Complete()
    {
        if (Status != OrderStatus.Assigned)
        {
            return Result.Failure(OrderErrors.CannotComplete);
        }

        Status = OrderStatus.Completed;
        RaiseDomainEvent(new OrderCompletedDomainEvent(Id));

        return Result.Success();
    }

    /// <summary>Business rule: si annulla solo un ordine Pending o Assigned.</summary>
    public Result Cancel()
    {
        if (Status is not (OrderStatus.Pending or OrderStatus.Assigned))
        {
            return Result.Failure(OrderErrors.CannotCancel);
        }

        Status = OrderStatus.Cancelled;
        AssignedHubId = null;
        RaiseDomainEvent(new OrderCancelledDomainEvent(Id));

        return Result.Success();
    }

    /// <summary>Ricostruisce l'aggregato dallo store senza eventi né regole.</summary>
    public static Order Rehydrate(
        Guid id,
        OrderNumber orderNumber,
        CustomerName customerName,
        OrderPriority priority,
        OrderStatus status,
        Guid? assignedHubId,
        DateTime placedOnUtc) =>
        new(id, orderNumber, customerName, priority, status, assignedHubId, placedOnUtc);
}
