using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Orders.Events;

public sealed record OrderCancelledDomainEvent(Guid OrderId) : DomainEvent;
