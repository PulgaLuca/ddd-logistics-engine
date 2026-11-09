using FluentValidation;
using LogisticsEngine.Domain.Users.ValueObjects;

namespace LogisticsEngine.Application.Users.RegisterUser;

public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    private const int _minimumPasswordLength = 12;

    public RegisterUserCommandValidator()
    {
        RuleFor(command => command.UserName)
            .Must(value => UserName.Create(value).IsSuccess)
            .WithMessage("Il nome utente deve contenere 3-64 caratteri alfanumerici, '.', '_' o '-'.");
        RuleFor(command => command.Password)
            .NotEmpty()
            .MinimumLength(_minimumPasswordLength)
            .WithMessage("La password deve contenere almeno 12 caratteri.");
    }
}
