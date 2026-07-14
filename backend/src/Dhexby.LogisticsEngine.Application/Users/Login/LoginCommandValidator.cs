using FluentValidation;

namespace Dhexby.LogisticsEngine.Application.Users.Login;

/// <summary>Validazione di forma della richiesta di login.</summary>
public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(command => command.Email).NotEmpty().WithMessage("The email is required.");
        RuleFor(command => command.Password).NotEmpty().WithMessage("The password is required.");
    }
}
