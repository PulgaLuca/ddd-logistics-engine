using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Orders.Events;

public sealed record OrderAssignedDomainEvent(Guid OrderId, Guid HubId) : DomainEvent;
