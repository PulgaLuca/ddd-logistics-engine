using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Shipments.CreateShipment;

public sealed record CreateShipmentCommand(
    string TrackingNumber,
    AddressDto Origin,
    AddressDto Destination,
    decimal WeightInKilograms,
    bool IsExpress) : IRequest<Result<Guid>>;
