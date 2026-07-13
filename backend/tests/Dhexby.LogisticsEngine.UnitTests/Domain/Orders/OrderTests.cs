using Dhexby.LogisticsEngine.Domain.Orders;
using Dhexby.LogisticsEngine.Domain.Orders.Events;
using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.UnitTests.Domain.Orders;

public class OrderTests
{
    private static Order CreatePendingOrder() => Order.Create(
        OrderNumber.Create("ORD-ABC12345").Value,
        CustomerName.Create("Mario Rossi").Value,
        OrderPriority.Express,
        DateTime.UtcNow).Value;

    [Fact]
    public void Create_ShouldStartPending_AndRaisePlacedEvent()
    {
        Order order = CreatePendingOrder();

        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.Null(order.AssignedHubId);
        Assert.Contains(order.DomainEvents, domainEvent => domainEvent is OrderPlacedDomainEvent);
    }

    [Fact]
    public void AssignToHub_ShouldMoveToAssigned_AndStoreHub()
    {
        Order order = CreatePendingOrder();
        Guid hubId = Guid.NewGuid();

        Result result = order.AssignToHub(hubId);

        Assert.True(result.IsSuccess);
        Assert.Equal(OrderStatus.Assigned, order.Status);
        Assert.Equal(hubId, order.AssignedHubId);
        Assert.Contains(order.DomainEvents, domainEvent =>
            domainEvent is OrderAssignedDomainEvent assigned && assigned.HubId == hubId);
    }

    [Fact]
    public void AssignToHub_ShouldFail_WhenHubIdIsEmpty()
    {
        Order order = CreatePendingOrder();

        Result result = order.AssignToHub(Guid.Empty);

        Assert.True(result.IsFailure);
        Assert.Equal(OrderErrors.HubIdRequired, result.Error);
    }

    [Fact]
    public void AssignToHub_ShouldFail_WhenNotPending()
    {
        Order order = CreatePendingOrder();
        order.AssignToHub(Guid.NewGuid());

        Result result = order.AssignToHub(Guid.NewGuid());

        Assert.True(result.IsFailure);
        Assert.Equal(OrderErrors.NotPending, result.Error);
    }

    [Fact]
    public void Unassign_ShouldRevertToPending_AsInverseOfAssign()
    {
        Order order = CreatePendingOrder();
        Guid hubId = Guid.NewGuid();
        order.AssignToHub(hubId);

        Result result = order.Unassign();

        Assert.True(result.IsSuccess);
        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.Null(order.AssignedHubId);
        Assert.Contains(order.DomainEvents, domainEvent =>
            domainEvent is OrderUnassignedDomainEvent unassigned && unassigned.PreviousHubId == hubId);
    }

    [Fact]
    public void Unassign_ShouldFail_WhenNotAssigned()
    {
        Order order = CreatePendingOrder();

        Result result = order.Unassign();

        Assert.True(result.IsFailure);
        Assert.Equal(OrderErrors.NotAssigned, result.Error);
    }

    [Fact]
    public void Complete_ShouldSucceed_WhenAssigned()
    {
        Order order = CreatePendingOrder();
        order.AssignToHub(Guid.NewGuid());

        Result result = order.Complete();

        Assert.True(result.IsSuccess);
        Assert.Equal(OrderStatus.Completed, order.Status);
        Assert.Contains(order.DomainEvents, domainEvent => domainEvent is OrderCompletedDomainEvent);
    }

    [Fact]
    public void Complete_ShouldFail_WhenPending()
    {
        Order order = CreatePendingOrder();

        Result result = order.Complete();

        Assert.True(result.IsFailure);
        Assert.Equal(OrderErrors.CannotComplete, result.Error);
    }

    [Fact]
    public void Cancel_ShouldSucceed_FromPending()
    {
        Order order = CreatePendingOrder();

        Result result = order.Cancel();

        Assert.True(result.IsSuccess);
        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    [Fact]
    public void Cancel_ShouldFail_WhenAlreadyCompleted()
    {
        Order order = CreatePendingOrder();
        order.AssignToHub(Guid.NewGuid());
        order.Complete();

        Result result = order.Cancel();

        Assert.True(result.IsFailure);
        Assert.Equal(OrderErrors.CannotCancel, result.Error);
    }
}
