using FluentValidation;

namespace Dhexby.LogisticsEngine.Application.Zones.AddStreetToZone;

/// <summary>Validazione della richiesta di aggiunta via a una zona.</summary>
public sealed class AddStreetToZoneCommandValidator : AbstractValidator<AddStreetToZoneCommand>
{
    public AddStreetToZoneCommandValidator()
    {
        RuleFor(command => command.ZoneId).NotEmpty().WithMessage("The zone id is required.");
        RuleFor(command => command.StreetName).NotEmpty().WithMessage("The street name is required.");
        RuleFor(command => command.PostalCode).NotEmpty().WithMessage("The postal code is required.");
    }
}
