using Dhexby.SharedKernel;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Abstractions.Behaviors;

/// <summary>
/// Pipeline behavior di MediatR: esegue i validator FluentValidation registrati
/// per la richiesta PRIMA dell'handler. In caso di violazioni non lancia
/// eccezioni ma ritorna un Result fallito (coerente con il Result pattern),
/// così il controller lo traduce in HTTP 400.
/// </summary>
/// <typeparam name="TRequest">Command o query in ingresso.</typeparam>
/// <typeparam name="TResponse">Deve essere un Result (o Result&lt;T&gt;).</typeparam>
public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
    where TResponse : Result
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next(cancellationToken);
        }

        var context = new ValidationContext<TRequest>(request);

        ValidationFailure[] failures = _validators
            .Select(validator => validator.Validate(context))
            .SelectMany(validationResult => validationResult.Errors)
            .Where(failure => failure is not null)
            .ToArray();

        if (failures.Length == 0)
        {
            return await next(cancellationToken);
        }

        return CreateValidationResult(ToValidationError(failures));
    }

    private static ValidationError ToValidationError(IEnumerable<ValidationFailure> failures)
    {
        IReadOnlyList<Error> errors = failures
            .Select(failure => Error.Validation(failure.PropertyName, failure.ErrorMessage))
            .ToArray();

        return new ValidationError(errors);
    }

    /// <summary>
    /// Costruisce l'istanza corretta di Result/Result&lt;T&gt; a partire dal tipo
    /// di risposta atteso: per Result&lt;T&gt; invoca la factory statica via reflection.
    /// </summary>
    private static TResponse CreateValidationResult(ValidationError error)
    {
        if (typeof(TResponse) == typeof(Result))
        {
            return (TResponse)(object)Result.Failure(error);
        }

        object validationResult = typeof(Result<>)
            .MakeGenericType(typeof(TResponse).GenericTypeArguments[0])
            .GetMethod(nameof(Result<object>.ValidationFailure))!
            .Invoke(null, [error])!;

        return (TResponse)validationResult;
    }
}
