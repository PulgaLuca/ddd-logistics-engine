using System.Text.RegularExpressions;
using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Users;

/// <summary>
/// Value Object: indirizzo email dell'utente, normalizzato in minuscolo.
/// Validazione di forma volutamente semplice (parte-locale@dominio.tld).
/// </summary>
public sealed partial class Email : ValueObject
{
    private Email(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<Email> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<Email>(UserErrors.EmailEmpty);
        }

        string normalized = value.Trim().ToLowerInvariant();

        if (!Format().IsMatch(normalized))
        {
            return Result.Failure<Email>(UserErrors.EmailInvalidFormat);
        }

        return new Email(normalized);
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex Format();
}
