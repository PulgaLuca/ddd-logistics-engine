using LogisticsEngine.Domain.Hubs.Errors;
using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.Domain.Shared.Names;

/// <summary>Value object che garantisce un nome normalizzato e valido.</summary>
public sealed class Name : ValueObject
{
    private const int _minimumLength = 1;
    private const int _maximumLength = 120;
    private Name(string value) => Value = value;
    public string Value { get; }

    public static Result<Name> Create(string? value)
    {
        string normalized = value?.Trim() ?? string.Empty;
        return normalized.Length < _minimumLength || normalized.Length > _maximumLength ? Result.Failure<Name>(NameErrors.InvalidName) : Result.Success(new Name(normalized));
    }

    protected override IEnumerable<object> GetAtomicValues() => [Value];
    public override string ToString() => Value;
}
