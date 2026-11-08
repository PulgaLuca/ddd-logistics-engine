using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using LogisticsEngine.Domain.Hubs.Errors;
using LogisticsEngine.Domain.Shared.Names;

namespace LogisticsEngine.Application.Hubs.GetBy;

public sealed class GetByNameCommandValidator : AbstractValidator<GetByNameCommand>
{
    public GetByNameCommandValidator() =>
        RuleFor(command => command.name).NotEmpty()
        .WithErrorCode(NameErrors.InvalidName.Code).WithMessage(NameErrors.InvalidName.Description)
            .Must(value => Name.Create(value).IsSuccess)
        .WithErrorCode(NameErrors.InvalidName.Code).WithMessage(NameErrors.InvalidName.Description);
}
