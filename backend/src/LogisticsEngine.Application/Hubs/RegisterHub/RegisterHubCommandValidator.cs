using FluentValidation;
using LogisticsEngine.Domain.Hubs.Errors;
using LogisticsEngine.Domain.Hubs.Vo;
using LogisticsEngine.Domain.Shared.Names;

namespace LogisticsEngine.Application.Hubs.RegisterHub;

public sealed class RegisterHubCommandValidator : AbstractValidator<RegisterHubCommand>
{
    public RegisterHubCommandValidator()
    {
        RuleFor(command => command.Name).NotEmpty()
            .WithErrorCode(NameErrors.InvalidName.Code).WithMessage(NameErrors.InvalidName.Description);

        RuleFor(command => command.Name).Must(value => Name.Create(value).IsSuccess)
            .WithErrorCode(NameErrors.InvalidName.Code).WithMessage(NameErrors.InvalidName.Description);

        RuleFor(command => command.Zone).Must(value => Zone.Create(value).IsSuccess)
            .WithErrorCode(HubErrors.InvalidZone.Code).WithMessage(HubErrors.InvalidZone.Description);
    }
}
