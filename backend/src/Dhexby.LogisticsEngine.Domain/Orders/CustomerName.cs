using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Orders;

/// <summary>Value Object: nome del cliente dell'ordine (1..200 caratteri).</summary>
public sealed class CustomerName : ValueObject
{
    public const int MaxLength = 200;

    private CustomerName(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<CustomerName> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<CustomerName>(OrderErrors.CustomerNameEmpty);
        }

        string trimmed = value.Trim();

        if (trimmed.Length > MaxLength)
        {
            return Result.Failure<CustomerName>(OrderErrors.CustomerNameTooLong);
        }

        return new CustomerName(trimmed);
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
