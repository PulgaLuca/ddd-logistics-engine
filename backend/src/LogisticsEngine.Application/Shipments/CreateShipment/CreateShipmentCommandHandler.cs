using LogisticsEngine.Application.Shared;
using LogisticsEngine.Domain.Shipments;
using LogisticsEngine.Domain.Shipments.Entities;
using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.Application.Shipments.CreateShipment;

public sealed class CreateShipmentCommandHandler(IShipmentRepository repository) : ICommandHandler<CreateShipmentCommand, Result<ShipmentDto>>
{
    public Result<ShipmentDto> Handle(CreateShipmentCommand command)
    {
        Result<Shipment> shipment = Shipment.Create(Guid.NewGuid(), command.TrackingNumber);
        if (shipment.IsFailure)
        {
            return Result.Failure<ShipmentDto>(shipment.Error);
        }

        repository.Add(shipment.Value);
        return Result.Success(ShipmentDto.From(shipment.Value));
    }
}
