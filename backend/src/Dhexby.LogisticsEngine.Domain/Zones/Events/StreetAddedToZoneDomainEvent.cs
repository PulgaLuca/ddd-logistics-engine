using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Zones.Events;

public sealed record StreetAddedToZoneDomainEvent(Guid ZoneId, string StreetName) : DomainEvent;
