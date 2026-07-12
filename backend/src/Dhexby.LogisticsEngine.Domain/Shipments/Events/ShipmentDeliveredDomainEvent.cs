using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Shipments.Events;

public sealed record ShipmentDeliveredDomainEvent(Guid ShipmentId) : DomainEvent;
