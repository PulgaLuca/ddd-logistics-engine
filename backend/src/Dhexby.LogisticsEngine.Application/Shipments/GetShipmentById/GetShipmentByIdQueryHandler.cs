using Dhexby.LogisticsEngine.Domain.Shipments;
using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Shipments.GetShipmentById;

/// <summary>Use case di lettura: ritorna la proiezione DTO della spedizione.</summary>
public sealed class GetShipmentByIdQueryHandler : IRequestHandler<GetShipmentByIdQuery, Result<ShipmentResponse>>
{
    private readonly IShipmentRepository _shipmentRepository;

    public GetShipmentByIdQueryHandler(IShipmentRepository shipmentRepository)
    {
        _shipmentRepository = shipmentRepository;
    }

    public async Task<Result<ShipmentResponse>> Handle(GetShipmentByIdQuery request, CancellationToken cancellationToken)
    {
        Shipment? shipment = await _shipmentRepository.GetByIdAsync(request.ShipmentId, cancellationToken);

        if (shipment is null)
            return Result.Failure<ShipmentResponse>(ShipmentErrors.NotFound(request.ShipmentId));

        return ShipmentResponse.FromDomain(shipment);
    }
}
