using Dhexby.LogisticsEngine.Domain.Shipments;

namespace Dhexby.LogisticsEngine.Console.Controllers;

/// <summary>Invoca la regola di pricing delle spedizioni registrata nel dominio.</summary>
public sealed class ShipmentPricingController
{
    private readonly IShipmentPricingService _pricingService;

    public ShipmentPricingController(IShipmentPricingService pricingService)
    {
        _pricingService = pricingService;
    }

    public void Calculate()
    {
        System.Console.Write("Peso in kg: ");
        if (!decimal.TryParse(
                System.Console.ReadLine(),
                System.Globalization.NumberStyles.Number,
                System.Globalization.CultureInfo.InvariantCulture,
                out decimal kilograms))
        {
            System.Console.WriteLine("Peso non valido.");
            return;
        }

        System.Console.Write("Express? (s/n): ");
        bool isExpress = string.Equals(System.Console.ReadLine()?.Trim(), "s", StringComparison.OrdinalIgnoreCase);

        var weight = Weight.Create(kilograms);
        if (weight.IsFailure)
        {
            System.Console.WriteLine($"{weight.Error.Code}: {weight.Error.Description}");
            return;
        }

        var price = _pricingService.CalculatePrice(weight.Value, isExpress);
        if (price.IsFailure)
        {
            System.Console.WriteLine($"{price.Error.Code}: {price.Error.Description}");
            return;
        }

        System.Console.WriteLine($"Prezzo: {price.Value.Amount:0.00} {price.Value.Currency}");
    }
}
