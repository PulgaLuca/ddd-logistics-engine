using Dhexby.LogisticsEngine.Domain.Orders;
using FluentValidation;

namespace Dhexby.LogisticsEngine.Application.Orders.PlaceOrder;

/// <summary>Validazione di forma della richiesta di creazione ordine.</summary>
public sealed class PlaceOrderCommandValidator : AbstractValidator<PlaceOrderCommand>
{
    public PlaceOrderCommandValidator()
    {
        RuleFor(command => command.OrderNumber)
            .NotEmpty().WithMessage("The order number is required.")
            .Matches("^ORD-[A-Za-z0-9]{8}$")
            .WithMessage("The order number must match the format ORD-XXXXXXXX (8 letters or digits).");

        RuleFor(command => command.CustomerName)
            .NotEmpty().WithMessage("The customer name is required.")
            .MaximumLength(CustomerName.MaxLength);

        RuleFor(command => command.Priority)
            .NotEmpty().WithMessage("The priority is required.")
            .Must(BeAValidPriority)
            .WithMessage("The priority must be one of: Standard, Express, Priority.");
    }

    private static bool BeAValidPriority(string priority) =>
        Enum.TryParse(priority, ignoreCase: true, out OrderPriority parsed) && Enum.IsDefined(parsed);
}
