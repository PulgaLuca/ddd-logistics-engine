using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Shipments;

/// <summary>
/// Regola di pricing: base fissa + tariffa al kg, maggiorata del 50% per l'express.
/// Servizio puro (senza dipendenze): facilmente testabile in isolamento.
/// </summary>
public sealed class ShipmentPricingService : IShipmentPricingService
{
    public const string Currency = "EUR";

    private const decimal _basePrice = 5.00m;
    private const decimal _pricePerKilogram = 1.20m;
    private const decimal _expressMultiplier = 1.5m;

    public Result<Money> CalculatePrice(Weight weight, bool isExpress)
    {
        decimal amount = _basePrice + (weight.Kilograms * _pricePerKilogram);

        if (isExpress)
            amount *= _expressMultiplier;

        return Money.Create(amount, Currency);
    }
}
