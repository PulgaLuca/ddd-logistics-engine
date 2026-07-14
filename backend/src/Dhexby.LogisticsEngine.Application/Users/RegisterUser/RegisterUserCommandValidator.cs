using Dhexby.LogisticsEngine.Domain.Users;
using FluentValidation;

namespace Dhexby.LogisticsEngine.Application.Users.RegisterUser;

/// <summary>Validazione di forma della richiesta di registrazione.</summary>
public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public const int MinPasswordLength = 8;

    public RegisterUserCommandValidator()
    {
        RuleFor(command => command.Email)
            .NotEmpty().WithMessage("The email is required.")
            .EmailAddress().WithMessage("The email format is not valid.");

        RuleFor(command => command.Password)
            .NotEmpty().WithMessage("The password is required.")
            .MinimumLength(MinPasswordLength)
            .WithMessage($"The password must be at least {MinPasswordLength} characters long.");

        RuleFor(command => command.Role)
            .NotEmpty().WithMessage("The role is required.")
            .Must(BeAValidRole)
            .WithMessage("The role must be one of: Administrator, Manager, Operator, Viewer.");
    }

    private static bool BeAValidRole(string role) =>
        Enum.TryParse(role, ignoreCase: true, out UserRole parsed) && Enum.IsDefined(parsed);
}
