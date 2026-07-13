using FluentValidation;

namespace Dhexby.LogisticsEngine.Application.Hubs.GetHubById;

/// <summary>Validazione della query: l'id hub deve essere presente.</summary>
public sealed class GetHubByIdQueryValidator : AbstractValidator<GetHubByIdQuery>
{
    public GetHubByIdQueryValidator()
    {
        RuleFor(query => query.HubId).NotEmpty().WithMessage("The hub id is required.");
    }
}
