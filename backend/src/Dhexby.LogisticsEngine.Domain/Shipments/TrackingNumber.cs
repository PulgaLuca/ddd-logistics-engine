using System.Text.RegularExpressions;
using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Shipments;

/// <summary>
/// Value Object: identifica una spedizione verso il mondo esterno.
/// Formato: TRK- seguito da 8 caratteri alfanumerici maiuscoli (es. TRK-A1B2C3D4).
/// </summary>
public sealed partial class TrackingNumber : ValueObject
{
    private TrackingNumber(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<TrackingNumber> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<TrackingNumber>(ShipmentErrors.TrackingNumberEmpty);
        }

        string normalized = value.Trim().ToUpperInvariant();

        if (!Format().IsMatch(normalized))
        {
            return Result.Failure<TrackingNumber>(ShipmentErrors.TrackingNumberInvalidFormat);
        }

        return new TrackingNumber(normalized);
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    [GeneratedRegex("^TRK-[A-Z0-9]{8}$")]
    private static partial Regex Format();
}
