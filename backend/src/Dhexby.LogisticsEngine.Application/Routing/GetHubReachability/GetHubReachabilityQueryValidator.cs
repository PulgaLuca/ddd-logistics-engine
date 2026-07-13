using FluentValidation;

namespace Dhexby.LogisticsEngine.Application.Routing.GetHubReachability;

/// <summary>Validazione della query: gli id di origine e destinazione sono richiesti.</summary>
public sealed class GetHubReachabilityQueryValidator : AbstractValidator<GetHubReachabilityQuery>
{
    public GetHubReachabilityQueryValidator()
    {
        RuleFor(query => query.OriginHubId).NotEmpty().WithMessage("The origin hub id is required.");
        RuleFor(query => query.DestinationHubId).NotEmpty().WithMessage("The destination hub id is required.");
    }
}
