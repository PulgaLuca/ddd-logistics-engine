using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.Domain.Users.ValueObjects;

/// <summary>Rappresenta esclusivamente una password già hashata, mai la password in chiaro.</summary>
public sealed class PasswordHash : ValueObject
{
    private PasswordHash(string value) => Value = value;

    public string Value { get; }

    public static Result<PasswordHash> Create(string? value) =>
        string.IsNullOrWhiteSpace(value)
            ? Result.Failure<PasswordHash>(UserErrors.InvalidPasswordHash)
            : Result.Success(new PasswordHash(value));

    protected override IEnumerable<object> GetAtomicValues() => [Value];
}
