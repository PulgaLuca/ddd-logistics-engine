using FluentValidation;
using LogisticsEngine.Domain.Hubs.Errors;
using LogisticsEngine.Domain.Shared.Names;

namespace LogisticsEngine.Application.Hubs.GetBy;

public sealed class GetByNameCommandValidator : AbstractValidator<GetHubByNameQuery>
{
    public GetByNameCommandValidator() =>
        RuleFor(command => command.Name).NotEmpty()
        .WithErrorCode(NameErrors.InvalidName.Code).WithMessage(NameErrors.InvalidName.Description)
            .Must(value => Name.Create(value).IsSuccess)
        .WithErrorCode(NameErrors.InvalidName.Code).WithMessage(NameErrors.InvalidName.Description);
}
