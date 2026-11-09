using LogisticsEngine.Domain.Shipments;
using LogisticsEngine.Domain.Shipments.Errors;
using LogisticsEngine.SharedKernel;
using MediatR;

namespace LogisticsEngine.Application.Shipments.UpdateStatus;

public sealed class UpdateShipmentStatusCommandHandler(IShipmentRepository repository)
    : IRequestHandler<UpdateShipmentStatusCommand, Result>
{
    public Task<Result> Handle(UpdateShipmentStatusCommand command, CancellationToken cancellationToken)
    {
        if (command.Id == Guid.Empty)
        {
            return Task.FromResult(Result.Failure(ShipmentErrors.InvalidId));
        }

        var shipment = repository.GetById(command.Id);
        if (shipment is null)
        {
            return Task.FromResult(Result.Failure(ShipmentErrors.NotFound(command.Id)));
        }

        Result transition = shipment.ChangeStatus(command.Status);
        if (transition.IsFailure)
        {
            return Task.FromResult(transition);
        }

        return Task.FromResult(repository.Update(shipment) ? Result.Success() : Result.Failure(ShipmentErrors.NotFound(command.Id)));
    }
}
