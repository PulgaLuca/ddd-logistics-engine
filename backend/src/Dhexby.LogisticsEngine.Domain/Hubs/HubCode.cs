using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Hubs;

/// <summary>
/// Value Object: codice identificativo dell'hub. Formato HUB-XXXX
/// (4 caratteri alfanumerici maiuscoli).
/// </summary>
public sealed class HubCode : ValueObject
{
    private HubCode(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<HubCode> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<HubCode>(HubErrors.CodeEmpty);

        string normalized = value.Trim().ToUpperInvariant();

        return new HubCode(normalized);
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
