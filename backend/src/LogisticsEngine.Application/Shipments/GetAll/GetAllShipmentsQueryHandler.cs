using LogisticsEngine.Domain.Shipments;
using MediatR;

namespace LogisticsEngine.Application.Shipments.GetAll;

public sealed class GetAllShipmentsQueryHandler(IShipmentRepository repository)
    : IRequestHandler<GetAllShipmentsQuery, IReadOnlyList<ShipmentDto>>
{
    public Task<IReadOnlyList<ShipmentDto>> Handle(GetAllShipmentsQuery query, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<ShipmentDto>>(repository.GetAll().Select(ShipmentDto.From).ToArray());
}
