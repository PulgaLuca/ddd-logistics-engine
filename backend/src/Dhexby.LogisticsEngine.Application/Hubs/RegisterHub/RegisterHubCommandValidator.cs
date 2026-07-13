using Dhexby.LogisticsEngine.Domain.Hubs;
using FluentValidation;

namespace Dhexby.LogisticsEngine.Application.Hubs.RegisterHub;

/// <summary>Validazione di forma della richiesta di registrazione hub.</summary>
public sealed class RegisterHubCommandValidator : AbstractValidator<RegisterHubCommand>
{
    public RegisterHubCommandValidator()
    {
        RuleFor(command => command.Code)
            .NotEmpty().WithMessage("The hub code is required.")
            .Matches("^HUB-[A-Za-z0-9]{4}$")
            .WithMessage("The hub code must match the format HUB-XXXX (4 letters or digits).");

        RuleFor(command => command.Name)
            .NotEmpty().WithMessage("The hub name is required.")
            .MaximumLength(Hub.MaxNameLength);

        RuleFor(command => command.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("The latitude must be between -90 and 90 degrees.");

        RuleFor(command => command.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("The longitude must be between -180 and 180 degrees.");
    }
}
