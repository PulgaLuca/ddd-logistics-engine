using LogisticsEngine.Domain.Shipments;
using LogisticsEngine.Domain.Shipments.Entities;
using LogisticsEngine.SharedKernel;
using MediatR;

namespace LogisticsEngine.Application.Shipments.CreateShipment;

public sealed class CreateShipmentCommandHandler(IShipmentRepository repository) : IRequestHandler<CreateShipmentCommand, Result<ShipmentDto>>
{
    public Task<Result<ShipmentDto>> Handle(CreateShipmentCommand command, CancellationToken cancellationToken)
    {
        Result<Shipment> shipment = Shipment.Create(Guid.NewGuid(), command.TrackingNumber);
        if (shipment.IsFailure)
        {
            return Task.FromResult(Result.Failure<ShipmentDto>(shipment.Error));
        }

        repository.Add(shipment.Value);
        return Task.FromResult(Result.Success(ShipmentDto.From(shipment.Value)));
    }
}
