using Dhexby.SharedKernel;

namespace Dhexby.LogisticsEngine.Domain.Shipments;

/// <summary>
/// Regola di pricing: base fissa + tariffa al kg, maggiorata del 50% per l'express.
/// Servizio puro (senza dipendenze): facilmente testabile in isolamento.
/// </summary>
public sealed class ShipmentPricingService : IShipmentPricingService
{
    public const string Currency = "EUR";

    private const decimal BasePrice = 5.00m;
    private const decimal PricePerKilogram = 1.20m;
    private const decimal ExpressMultiplier = 1.5m;

    public Result<Money> CalculatePrice(Weight weight, bool isExpress)
    {
        decimal amount = BasePrice + (weight.Kilograms * PricePerKilogram);

        if (isExpress)
        {
            amount *= ExpressMultiplier;
        }

        return Money.Create(amount, Currency);
    }
}
