using Dhexby.LogisticsEngine.Domain.Zones;
using FluentValidation;

namespace Dhexby.LogisticsEngine.Application.Zones.CreateZone;

/// <summary>Validazione di forma della richiesta di creazione zona.</summary>
public sealed class CreateZoneCommandValidator : AbstractValidator<CreateZoneCommand>
{
    public CreateZoneCommandValidator()
    {
        RuleFor(command => command.Code)
            .NotEmpty().WithMessage("The zone code is required.")
            .Matches("^ZON-[A-Za-z0-9]{4}$")
            .WithMessage("The zone code must match the format ZON-XXXX (4 letters or digits).");

        RuleFor(command => command.Name)
            .NotEmpty().WithMessage("The zone name is required.")
            .MaximumLength(Zone.MaxNameLength);

        RuleFor(command => command.ParentZoneId)
            .NotEqual(Guid.Empty).WithMessage("The parent zone id, when provided, must be a valid id.")
            .When(command => command.ParentZoneId.HasValue);
    }
}
