using LogisticsEngine.Application.Shared;
using LogisticsEngine.Domain.Shipments;
using LogisticsEngine.Domain.Shipments.Errors;
using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.Application.Shipments.Delete;

public sealed class DeleteShipmentCommandHandler(IShipmentRepository repository)
    : ICommandHandler<DeleteShipmentCommand, Result>
{
    public Result Handle(DeleteShipmentCommand command)
    {
        if (command.Id == Guid.Empty)
        {
            return Result.Failure(ShipmentErrors.InvalidId);
        }

        return repository.Delete(command.Id)
            ? Result.Success()
            : Result.Failure(ShipmentErrors.NotFound(command.Id));
    }
}
