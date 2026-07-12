using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Shipments;

/// <summary>
/// Value Object: importo monetario con valuta (codice ISO 4217 a 3 lettere).
/// </summary>
public sealed class Money : ValueObject
{
    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public decimal Amount { get; }

    public string Currency { get; }

    public static Result<Money> Create(decimal amount, string? currency)
    {
        if (amount < 0)
        {
            return Result.Failure<Money>(ShipmentErrors.MoneyNegative);
        }

        if (currency is null || currency.Trim().Length != 3)
        {
            return Result.Failure<Money>(ShipmentErrors.MoneyInvalidCurrency);
        }

        return new Money(decimal.Round(amount, 2), currency.Trim().ToUpperInvariant());
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Amount;
        yield return Currency;
    }
}
