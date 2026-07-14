using FluentValidation;

namespace Dhexby.LogisticsEngine.Application.Shipments.GetShipmentById;

/// <summary>Validazione della query: l'id richiesto deve essere presente.</summary>
public sealed class GetShipmentByIdQueryValidator : AbstractValidator<GetShipmentByIdQuery>
{
    public GetShipmentByIdQueryValidator()
    {
        RuleFor(query => query.ShipmentId)
            .NotEmpty().WithMessage("The shipment id is required.");
    }
}
