using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Shipments.Events;

public sealed record ShipmentCreatedDomainEvent(Guid ShipmentId) : DomainEvent;
