using FluentValidation;

namespace Dhexby.LogisticsEngine.Application.Zones.GetZoneById;

/// <summary>Validazione della query: l'id zona deve essere presente.</summary>
public sealed class GetZoneByIdQueryValidator : AbstractValidator<GetZoneByIdQuery>
{
    public GetZoneByIdQueryValidator()
    {
        RuleFor(query => query.ZoneId).NotEmpty().WithMessage("The zone id is required.");
    }
}
