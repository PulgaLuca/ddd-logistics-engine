using Dhexby.LogisticsEngine.Domain.Shipments;
using FluentValidation;

namespace Dhexby.LogisticsEngine.Application.Shipments.CreateShipment;

/// <summary>
/// Validazione di forma della richiesta (fail-fast al confine dell'Application):
/// intercetta l'input malformato prima che raggiunga il dominio. I Value Object
/// del Domain restano comunque la fonte di verità delle regole di business.
/// </summary>
public sealed class CreateShipmentCommandValidator : AbstractValidator<CreateShipmentCommand>
{
    public CreateShipmentCommandValidator()
    {
        RuleFor(command => command.TrackingNumber)
            .NotEmpty().WithMessage("The tracking number is required.")
            .Matches("^TRK-[A-Za-z0-9]{8}$")
            .WithMessage("The tracking number must match the format TRK-XXXXXXXX (8 letters or digits).");

        RuleFor(command => command.Origin)
            .NotNull().WithMessage("The origin address is required.")
            .SetValidator(new AddressDtoValidator());

        RuleFor(command => command.Destination)
            .NotNull().WithMessage("The destination address is required.")
            .SetValidator(new AddressDtoValidator());

        RuleFor(command => command.WeightInKilograms)
            .GreaterThan(0).WithMessage("The weight must be greater than zero.")
            .LessThanOrEqualTo(Weight.MaxKilograms)
            .WithMessage($"The weight cannot exceed {Weight.MaxKilograms} kg.");
    }
}

/// <summary>Validazione dei campi di un indirizzo (riusata per origine e destinazione, DRY).</summary>
public sealed class AddressDtoValidator : AbstractValidator<AddressDto>
{
    public AddressDtoValidator()
    {
        RuleFor(address => address.Street).NotEmpty().WithMessage("The street is required.");
        RuleFor(address => address.City).NotEmpty().WithMessage("The city is required.");
        RuleFor(address => address.PostalCode).NotEmpty().WithMessage("The postal code is required.");
        RuleFor(address => address.Country).NotEmpty().WithMessage("The country is required.");
    }
}
