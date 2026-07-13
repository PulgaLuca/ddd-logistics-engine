using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Zones.Events;

public sealed record ZoneCreatedDomainEvent(Guid ZoneId, Guid? ParentZoneId) : DomainEvent;
