using FluentValidation;
using LogisticsEngine.SharedKernel;
using MediatR;

namespace LogisticsEngine.Application;

/// <summary>Valida con FluentValidation tutte le richieste MediatR che restituiscono Result.</summary>
public sealed class ValidationBehavior<TRequest, TValue>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, Result<TValue>>
    where TRequest : notnull, IRequest<Result<TValue>>
{
    public async Task<Result<TValue>> Handle(
        TRequest request,
        RequestHandlerDelegate<Result<TValue>> next,
        CancellationToken cancellationToken)
    {
        IValidator<TRequest>[] registeredValidators = validators.ToArray();
        if (registeredValidators.Length == 0)
        {
            return await next(cancellationToken);
        }

        var context = new ValidationContext<TRequest>(request);
        var failures = new List<string>();
        foreach (IValidator<TRequest> validator in registeredValidators)
        {
            FluentValidation.Results.ValidationResult result =
                await validator.ValidateAsync(context, cancellationToken);
            failures.AddRange(result.Errors
            .Where(failure => failure is not null)
            .Select(failure => $"{failure.PropertyName}: {failure.ErrorMessage}"));
        }

        string[] distinctFailures = failures.Distinct(StringComparer.Ordinal).ToArray();
        return distinctFailures.Length == 0
            ? await next(cancellationToken)
            : Result.Failure<TValue>(Error.Validation("Validation.Failed", string.Join("; ", distinctFailures)));
    }
}
