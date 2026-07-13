using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Orders.Events;

public sealed record OrderPlacedDomainEvent(Guid OrderId) : DomainEvent;
