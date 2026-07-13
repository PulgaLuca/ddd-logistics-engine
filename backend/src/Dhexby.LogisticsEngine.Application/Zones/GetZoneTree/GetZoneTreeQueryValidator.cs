using FluentValidation;

namespace Dhexby.LogisticsEngine.Application.Zones.GetZoneTree;

/// <summary>Validazione della query: l'id della zona radice è richiesto.</summary>
public sealed class GetZoneTreeQueryValidator : AbstractValidator<GetZoneTreeQuery>
{
    public GetZoneTreeQueryValidator()
    {
        RuleFor(query => query.RootZoneId).NotEmpty().WithMessage("The root zone id is required.");
    }
}
