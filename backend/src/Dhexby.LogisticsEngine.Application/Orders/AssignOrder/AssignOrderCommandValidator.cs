using FluentValidation;

namespace Dhexby.LogisticsEngine.Application.Orders.AssignOrder;

/// <summary>Validazione della richiesta di assegnazione: id ordine e hub richiesti.</summary>
public sealed class AssignOrderCommandValidator : AbstractValidator<AssignOrderCommand>
{
    public AssignOrderCommandValidator()
    {
        RuleFor(command => command.OrderId).NotEmpty().WithMessage("The order id is required.");
        RuleFor(command => command.HubId).NotEmpty().WithMessage("The hub id is required.");
    }
}
