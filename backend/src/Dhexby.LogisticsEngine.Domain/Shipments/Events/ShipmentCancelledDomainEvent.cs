using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Shipments.Events;

public sealed record ShipmentCancelledDomainEvent(Guid ShipmentId) : DomainEvent;
