using FluentValidation;
using LogisticsEngine.Domain.Hubs.Errors;

namespace LogisticsEngine.Application.Hubs.DeleteHub;

public sealed class DeleteHubCommandValidator : AbstractValidator<DeleteHubCommand>
{
    public DeleteHubCommandValidator()
    {
        RuleFor(command => command.Id).NotEmpty()
            .WithErrorCode(HubErrors.InvalidId.Code).WithMessage(HubErrors.InvalidId.Description);
    }
}
