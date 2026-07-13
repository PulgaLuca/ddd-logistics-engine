using Dhexby.LogisticsEngine.Domain.Routing;
using FluentValidation;

namespace Dhexby.LogisticsEngine.Application.Routing.AddRoad;

/// <summary>Validazione di forma della richiesta di aggiunta strada.</summary>
public sealed class AddRoadCommandValidator : AbstractValidator<AddRoadCommand>
{
    public AddRoadCommandValidator()
    {
        RuleFor(command => command.FromHubId).NotEmpty().WithMessage("The origin hub id is required.");
        RuleFor(command => command.ToHubId).NotEmpty().WithMessage("The destination hub id is required.");

        RuleFor(command => command.ToHubId)
            .NotEqual(command => command.FromHubId)
            .WithMessage("A road must connect two different hubs.");

        RuleFor(command => command.DistanceInKilometers)
            .GreaterThan(0).WithMessage("The road distance must be greater than zero.");

        RuleFor(command => command.TrafficFactor)
            .InclusiveBetween(Road.MinTrafficFactor, Road.MaxTrafficFactor)
            .WithMessage($"The traffic factor must be between {Road.MinTrafficFactor} and {Road.MaxTrafficFactor}.");
    }
}
