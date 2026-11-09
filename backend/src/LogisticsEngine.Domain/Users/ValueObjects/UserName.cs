using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.Domain.Users.ValueObjects;

/// <summary>Nome utente normalizzato per confronti indipendenti da maiuscole e spazi esterni.</summary>
public sealed class UserName : ValueObject
{
    private const int MinimumLength = 3;
    private const int MaximumLength = 64;

    private UserName(string value) => Value = value;

    public string Value { get; }

    public static Result<UserName> Create(string? value)
    {
        string normalized = value?.Trim().ToLowerInvariant() ?? string.Empty;
        if (normalized.Length < MinimumLength || normalized.Length > MaximumLength ||
            normalized.Any(character => !char.IsAsciiLetterOrDigit(character) && character is not '.' and not '_' and not '-'))
        {
            return Result.Failure<UserName>(UserErrors.InvalidUserName);
        }

        return Result.Success(new UserName(normalized));
    }

    protected override IEnumerable<object> GetAtomicValues() => [Value];
    public override string ToString() => Value;
}
