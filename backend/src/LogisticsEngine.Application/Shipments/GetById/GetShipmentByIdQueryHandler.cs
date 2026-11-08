using LogisticsEngine.Application.Shared;
using LogisticsEngine.Domain.Shipments;
using LogisticsEngine.Domain.Shipments.Errors;
using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.Application.Shipments.GetById;

public sealed class GetShipmentByIdQueryHandler(IShipmentRepository repository)
    : ICommandHandler<GetShipmentByIdQuery, Result<ShipmentDto>>
{
    public Result<ShipmentDto> Handle(GetShipmentByIdQuery query)
    {
        if (query.Id == Guid.Empty)
        {
            return Result.Failure<ShipmentDto>(ShipmentErrors.InvalidId);
        }

        var shipment = repository.GetById(query.Id);
        return shipment is null
            ? Result.Failure<ShipmentDto>(ShipmentErrors.NotFound(query.Id))
            : Result.Success(ShipmentDto.From(shipment));
    }
}
