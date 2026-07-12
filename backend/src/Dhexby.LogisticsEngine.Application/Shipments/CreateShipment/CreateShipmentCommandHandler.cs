using Dhexby.LogisticsEngine.Domain.Shipments;
using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Shipments.CreateShipment;

/// <summary>
/// Use case: crea una spedizione. Orchestra Value Object, domain service
/// e repository; il flusso fallisce al primo Result negativo (Result pattern).
/// </summary>
public sealed class CreateShipmentCommandHandler : IRequestHandler<CreateShipmentCommand, Result<Guid>>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IShipmentPricingService _pricingService;

    public CreateShipmentCommandHandler(
        IShipmentRepository shipmentRepository,
        IShipmentPricingService pricingService)
    {
        _shipmentRepository = shipmentRepository;
        _pricingService = pricingService;
    }

    public async Task<Result<Guid>> Handle(CreateShipmentCommand request, CancellationToken cancellationToken)
    {
        Result<TrackingNumber> trackingNumberResult = TrackingNumber.Create(request.TrackingNumber);
        if (trackingNumberResult.IsFailure)
        {
            return Result.Failure<Guid>(trackingNumberResult.Error);
        }

        Result<Address> originResult = CreateAddress(request.Origin);
        if (originResult.IsFailure)
        {
            return Result.Failure<Guid>(originResult.Error);
        }

        Result<Address> destinationResult = CreateAddress(request.Destination);
        if (destinationResult.IsFailure)
        {
            return Result.Failure<Guid>(destinationResult.Error);
        }

        Result<Weight> weightResult = Weight.Create(request.WeightInKilograms);
        if (weightResult.IsFailure)
        {
            return Result.Failure<Guid>(weightResult.Error);
        }

        bool trackingNumberInUse = await _shipmentRepository.ExistsByTrackingNumberAsync(
            trackingNumberResult.Value,
            cancellationToken);

        if (trackingNumberInUse)
        {
            return Result.Failure<Guid>(ShipmentErrors.TrackingNumberAlreadyInUse);
        }

        Result<Money> priceResult = _pricingService.CalculatePrice(weightResult.Value, request.IsExpress);
        if (priceResult.IsFailure)
        {
            return Result.Failure<Guid>(priceResult.Error);
        }

        Result<Shipment> shipmentResult = Shipment.Create(
            trackingNumberResult.Value,
            originResult.Value,
            destinationResult.Value,
            weightResult.Value,
            priceResult.Value,
            request.IsExpress,
            DateTime.UtcNow);

        if (shipmentResult.IsFailure)
        {
            return Result.Failure<Guid>(shipmentResult.Error);
        }

        await _shipmentRepository.AddAsync(shipmentResult.Value, cancellationToken);

        return shipmentResult.Value.Id;
    }

    private static Result<Address> CreateAddress(AddressDto dto) =>
        Address.Create(dto.Street, dto.City, dto.PostalCode, dto.Country);
}
