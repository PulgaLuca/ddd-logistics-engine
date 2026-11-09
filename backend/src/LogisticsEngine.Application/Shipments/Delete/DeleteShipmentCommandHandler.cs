using LogisticsEngine.Domain.Shipments;
using LogisticsEngine.Domain.Shipments.Errors;
using LogisticsEngine.SharedKernel;
using MediatR;

namespace LogisticsEngine.Application.Shipments.Delete;

public sealed class DeleteShipmentCommandHandler(IShipmentRepository repository)
    : IRequestHandler<DeleteShipmentCommand, Result>
{
    public Task<Result> Handle(DeleteShipmentCommand command, CancellationToken cancellationToken)
    {
        if (command.Id == Guid.Empty)
        {
            return Task.FromResult(Result.Failure(ShipmentErrors.InvalidId));
        }

        return Task.FromResult(repository.Delete(command.Id)
            ? Result.Success()
            : Result.Failure(ShipmentErrors.NotFound(command.Id)));
    }
}
