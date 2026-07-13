using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Orders.Events;

public sealed record OrderCompletedDomainEvent(Guid OrderId) : DomainEvent;
