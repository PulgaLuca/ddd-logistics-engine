using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Hubs.Events;

public sealed record HubDeactivatedDomainEvent(Guid HubId) : DomainEvent;
