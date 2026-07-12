using Dhexby.LogisticsEngine.Domain.Shipments;
using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Shipments.DispatchShipment;

/// <summary>Use case: mette in transito una spedizione esistente.</summary>
public sealed class DispatchShipmentCommandHandler : IRequestHandler<DispatchShipmentCommand, Result>
{
    private readonly IShipmentRepository _shipmentRepository;

    public DispatchShipmentCommandHandler(IShipmentRepository shipmentRepository)
    {
        _shipmentRepository = shipmentRepository;
    }

    public async Task<Result> Handle(DispatchShipmentCommand request, CancellationToken cancellationToken)
    {
        Shipment? shipment = await _shipmentRepository.GetByIdAsync(request.ShipmentId, cancellationToken);

        if (shipment is null)
            return Result.Failure(ShipmentErrors.NotFound(request.ShipmentId));

        Result dispatchResult = shipment.Dispatch();
        if (dispatchResult.IsFailure)
            return dispatchResult;

        await _shipmentRepository.UpdateAsync(shipment, cancellationToken);

        return Result.Success();
    }
}
