using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Shipments.Events;

public sealed record ShipmentDispatchedDomainEvent(Guid ShipmentId) : DomainEvent;
