using Dhexby.LogisticsEngine.Application.Abstractions.Messaging;

namespace Dhexby.LogisticsEngine.Application.Shipments.CreateShipment;

public sealed record CreateShipmentCommand(
    string TrackingNumber,
    AddressDto Origin,
    AddressDto Destination,
    decimal WeightInKilograms,
    bool IsExpress) : ICommand<Guid>;
