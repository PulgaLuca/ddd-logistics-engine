using LogisticsEngine.Application.Shared;
using LogisticsEngine.Domain.Shipments;
using LogisticsEngine.Domain.Shipments.Errors;
using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.Application.Shipments.UpdateStatus;

public sealed class UpdateShipmentStatusCommandHandler(IShipmentRepository repository)
    : ICommandHandler<UpdateShipmentStatusCommand, Result>
{
    public Result Handle(UpdateShipmentStatusCommand command)
    {
        if (command.Id == Guid.Empty)
        {
            return Result.Failure(ShipmentErrors.InvalidId);
        }

        var shipment = repository.GetById(command.Id);
        if (shipment is null)
        {
            return Result.Failure(ShipmentErrors.NotFound(command.Id));
        }

        Result transition = shipment.ChangeStatus(command.Status);
        if (transition.IsFailure)
        {
            return transition;
        }

        return repository.Update(shipment) ? Result.Success() : Result.Failure(ShipmentErrors.NotFound(command.Id));
    }
}
