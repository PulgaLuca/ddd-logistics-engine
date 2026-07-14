using FluentValidation;

namespace Dhexby.LogisticsEngine.Application.Shipments.DispatchShipment;

/// <summary>Validazione della richiesta di dispatch: l'id deve essere presente.</summary>
public sealed class DispatchShipmentCommandValidator : AbstractValidator<DispatchShipmentCommand>
{
    public DispatchShipmentCommandValidator()
    {
        RuleFor(command => command.ShipmentId)
            .NotEmpty().WithMessage("The shipment id is required.");
    }
}
