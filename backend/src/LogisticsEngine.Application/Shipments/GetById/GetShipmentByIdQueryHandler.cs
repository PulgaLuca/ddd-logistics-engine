using LogisticsEngine.Domain.Shipments;
using LogisticsEngine.Domain.Shipments.Errors;
using LogisticsEngine.SharedKernel;
using MediatR;

namespace LogisticsEngine.Application.Shipments.GetById;

public sealed class GetShipmentByIdQueryHandler(IShipmentRepository repository)
    : IRequestHandler<GetShipmentByIdQuery, Result<ShipmentDto>>
{
    public Task<Result<ShipmentDto>> Handle(GetShipmentByIdQuery query, CancellationToken cancellationToken)
    {
        if (query.Id == Guid.Empty)
        {
            return Task.FromResult(Result.Failure<ShipmentDto>(ShipmentErrors.InvalidId));
        }

        var shipment = repository.GetById(query.Id);
        return shipment is null
            ? Task.FromResult(Result.Failure<ShipmentDto>(ShipmentErrors.NotFound(query.Id)))
            : Task.FromResult(Result.Success(ShipmentDto.From(shipment)));
    }
}
