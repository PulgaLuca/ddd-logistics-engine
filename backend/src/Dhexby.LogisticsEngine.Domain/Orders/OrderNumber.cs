using System.Text.RegularExpressions;
using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Orders;

/// <summary>
/// Value Object: identificativo esterno dell'ordine. Formato ORD-XXXXXXXX
/// (8 caratteri alfanumerici maiuscoli).
/// </summary>
public sealed partial class OrderNumber : ValueObject
{
    private OrderNumber(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<OrderNumber> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<OrderNumber>(OrderErrors.OrderNumberEmpty);
        }

        string normalized = value.Trim().ToUpperInvariant();

        if (!Format().IsMatch(normalized))
        {
            return Result.Failure<OrderNumber>(OrderErrors.OrderNumberInvalidFormat);
        }

        return new OrderNumber(normalized);
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    [GeneratedRegex("^ORD-[A-Z0-9]{8}$")]
    private static partial Regex Format();
}
