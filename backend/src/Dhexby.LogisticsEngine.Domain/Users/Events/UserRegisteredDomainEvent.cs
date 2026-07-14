using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Users.Events;

public sealed record UserRegisteredDomainEvent(Guid UserId, UserRole Role) : DomainEvent;
