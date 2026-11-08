using FluentValidation.Results;
using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.Application.Hubs.Validation;

internal static class HubValidation
{
    public static Error ToError(ValidationResult validationResult)
    {
        ValidationFailure failure = validationResult.Errors[0];
        return Error.Validation(failure.ErrorCode, failure.ErrorMessage);
    }
}
