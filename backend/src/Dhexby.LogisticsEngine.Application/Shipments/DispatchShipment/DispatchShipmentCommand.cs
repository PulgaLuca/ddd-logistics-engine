using Dhexby.LogisticsEngine.Application.Abstractions.Messaging;

namespace Dhexby.LogisticsEngine.Application.Shipments.DispatchShipment;

public sealed record DispatchShipmentCommand(Guid ShipmentId) : ICommand;
