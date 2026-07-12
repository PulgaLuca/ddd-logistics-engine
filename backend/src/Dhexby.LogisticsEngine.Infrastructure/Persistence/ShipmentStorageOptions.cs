namespace Dhexby.LogisticsEngine.Infrastructure.Persistence;

/// <summary>Opzioni di configurazione dello store JSON (sezione "Storage").</summary>
public sealed class ShipmentStorageOptions
{
    public const string SectionName = "Storage";

    public string ShipmentsFilePath { get; set; } = Path.Combine("data", "shipments.json");
}
