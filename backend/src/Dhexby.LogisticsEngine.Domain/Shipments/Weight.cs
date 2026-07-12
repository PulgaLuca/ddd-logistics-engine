using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Shipments;

/// <summary>
/// Value Object: peso della spedizione in chilogrammi.
/// Business rule: 0 &lt; peso &lt;= 1000 kg.
/// </summary>
public sealed class Weight : ValueObject
{
    public const decimal MaxKilograms = 1000m;

    private Weight(decimal kilograms)
    {
        Kilograms = kilograms;
    }

    public decimal Kilograms { get; }

    public static Result<Weight> Create(decimal kilograms)
    {
        if (kilograms <= 0)
            return Result.Failure<Weight>(ShipmentErrors.WeightNotPositive);

        if (kilograms > MaxKilograms)
            return Result.Failure<Weight>(ShipmentErrors.WeightTooHeavy);

        return new Weight(kilograms);
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Kilograms;
    }
}
