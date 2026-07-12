using Dhexby.LogisticsEngine.Domain.Shipments;

namespace Dhexby.LogisticsEngine.UnitTests.Domain.Shipments;

/// <summary>
/// Object mother condiviso dai test (DRY): un solo punto in cui costruire
/// oggetti di dominio validi.
/// </summary>
internal static class ShipmentTestData
{
    public const string ValidTrackingNumber = "TRK-ABC12345";

    public static TrackingNumber CreateTrackingNumber(string value = ValidTrackingNumber) =>
        TrackingNumber.Create(value).Value;

    public static Address CreateOrigin() =>
        Address.Create("Via Roma 1", "Milano", "20100", "IT").Value;

    public static Address CreateDestination() =>
        Address.Create("Via Garibaldi 2", "Torino", "10121", "IT").Value;

    public static Weight CreateWeight(decimal kilograms = 10m) =>
        Weight.Create(kilograms).Value;

    public static Money CreatePrice(decimal amount = 17m) =>
        Money.Create(amount, "EUR").Value;

    public static Shipment CreateShipment(bool isExpress = false) =>
        Shipment.Create(
            CreateTrackingNumber(),
            CreateOrigin(),
            CreateDestination(),
            CreateWeight(),
            CreatePrice(),
            isExpress,
            DateTime.UtcNow).Value;
}
