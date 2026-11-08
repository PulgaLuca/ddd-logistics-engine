using LogisticsEngine.Application.Shared;
using LogisticsEngine.Domain.Shipments;

namespace LogisticsEngine.Application.Shipments.GetAll;

public sealed class GetAllShipmentsQueryHandler(IShipmentRepository repository)
    : ICommandHandler<GetAllShipmentsQuery, IReadOnlyList<ShipmentDto>>
{
    public IReadOnlyList<ShipmentDto> Handle(GetAllShipmentsQuery query) =>
        repository.GetAll().Select(ShipmentDto.From).ToArray();
}
