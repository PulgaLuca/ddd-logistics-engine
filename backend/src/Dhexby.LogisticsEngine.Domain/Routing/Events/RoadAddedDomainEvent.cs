using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Routing.Events;

public sealed record RoadAddedDomainEvent(Guid RoadId, Guid FromHubId, Guid ToHubId) : DomainEvent;
