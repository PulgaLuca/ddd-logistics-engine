using FluentValidation;

namespace Dhexby.LogisticsEngine.Application.Routing.GetShortestRoute;

/// <summary>Validazione della query: gli id di origine e destinazione sono richiesti.</summary>
public sealed class GetShortestRouteQueryValidator : AbstractValidator<GetShortestRouteQuery>
{
    public GetShortestRouteQueryValidator()
    {
        RuleFor(query => query.OriginHubId).NotEmpty().WithMessage("The origin hub id is required.");
        RuleFor(query => query.DestinationHubId).NotEmpty().WithMessage("The destination hub id is required.");
    }
}
