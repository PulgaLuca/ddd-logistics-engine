using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Hubs.Events;

public sealed record HubRegisteredDomainEvent(Guid HubId) : DomainEvent;
