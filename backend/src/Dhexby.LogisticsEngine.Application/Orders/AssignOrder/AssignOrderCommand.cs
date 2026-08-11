using Dhexby.LogisticsEngine.Application.Abstractions.Messaging;

namespace Dhexby.LogisticsEngine.Application.Orders.AssignOrder;

public sealed record AssignOrderCommand(Guid OrderId, Guid HubId) : ICommand;
