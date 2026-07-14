using System.Text.RegularExpressions;
using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Zones;

/// <summary>
/// Value Object: codice della zona di consegna. Formato ZON-XXXX
/// (4 caratteri alfanumerici maiuscoli).
/// </summary>
public sealed partial class ZoneCode : ValueObject
{
    private ZoneCode(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<ZoneCode> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<ZoneCode>(ZoneErrors.CodeEmpty);
        }

        string normalized = value.Trim().ToUpperInvariant();

        if (!Format().IsMatch(normalized))
        {
            return Result.Failure<ZoneCode>(ZoneErrors.CodeInvalidFormat);
        }

        return new ZoneCode(normalized);
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    [GeneratedRegex("^ZON-[A-Z0-9]{4}$")]
    private static partial Regex Format();
}
