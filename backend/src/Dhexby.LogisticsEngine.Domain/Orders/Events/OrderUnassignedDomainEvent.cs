using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Orders.Events;

public sealed record OrderUnassignedDomainEvent(Guid OrderId, Guid PreviousHubId) : DomainEvent;
