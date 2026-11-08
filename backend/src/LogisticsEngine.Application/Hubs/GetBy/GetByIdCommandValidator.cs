using FluentValidation;
using LogisticsEngine.Application.Hubs.GetById;
using LogisticsEngine.Domain.Hubs.Errors;

namespace LogisticsEngine.Application.Hubs.GetBy;

public sealed class GetByIdCommandValidator : AbstractValidator<GetByIdCommand>
{
    public GetByIdCommandValidator() =>
        RuleFor(command => command.Id).NotEmpty()
        .WithErrorCode(HubErrors.InvalidId.Code).WithMessage(HubErrors.InvalidId.Description);
}
