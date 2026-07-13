using FluentValidation;

namespace Dhexby.LogisticsEngine.Application.Orders.GetOrderById;

/// <summary>Validazione della query: l'id ordine deve essere presente.</summary>
public sealed class GetOrderByIdQueryValidator : AbstractValidator<GetOrderByIdQuery>
{
    public GetOrderByIdQueryValidator()
    {
        RuleFor(query => query.OrderId).NotEmpty().WithMessage("The order id is required.");
    }
}
