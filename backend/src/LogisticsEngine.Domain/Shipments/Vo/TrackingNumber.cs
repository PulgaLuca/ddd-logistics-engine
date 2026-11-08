using LogisticsEngine.Domain.Shipments.Errors;
using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.Domain.Shipments.ValueObjects;

/// <summary>Identificativo di tracciamento normalizzato e validato alla creazione.</summary>
public sealed class TrackingNumber : ValueObject
{
    private const int _minimumLength = 8;
    private const int _maximumLength = 24;
    private TrackingNumber(string value) => Value = value;
    public string Value { get; }

    public static Result<TrackingNumber> Create(string? value)
    {
        string normalized = value?.Trim().ToUpperInvariant() ?? string.Empty;
        return normalized.Length < _minimumLength || normalized.Length > _maximumLength || normalized.Any(character => !char.IsAsciiLetterOrDigit(character))
            ? Result.Failure<TrackingNumber>(ShipmentErrors.InvalidTrackingNumber)
            : Result.Success(new TrackingNumber(normalized));
    }

    protected override IEnumerable<object> GetAtomicValues() => [Value];
    public override string ToString() => Value;
}
