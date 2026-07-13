namespace Dhexby.SharedKernel
{
    /// <summary>
    /// Errore composito: aggrega più errori di validazione in un unico Error,
    /// così il Result pattern può trasportare tutte le violazioni insieme.
    /// </summary>
    public sealed record ValidationError : Error
    {
        public ValidationError(IReadOnlyList<Error> errors)
            : base("Validation.General", "One or more validation errors occurred", ErrorType.Validation)
        {
            Errors = errors;
        }

        public IReadOnlyList<Error> Errors { get; }

        public static ValidationError FromResults(IEnumerable<Result> results) =>
            new(results.Where(result => result.IsFailure).Select(result => result.Error).ToArray());
    }
}
