namespace Dhexby.LogisticsEngine.Infrastructure.Persistence;

/// <summary>
/// Opzioni di configurazione degli store JSON (sezione "Storage"): un file
/// per aggregate root.
/// </summary>
public sealed class JsonStorageOptions
{
    public const string SectionName = "Storage";

    public string ShipmentsFilePath { get; set; } = Path.Combine("data", "shipments.json");

    public string OrdersFilePath { get; set; } = Path.Combine("data", "orders.json");

    public string HubsFilePath { get; set; } = Path.Combine("data", "hubs.json");

    public string ZonesFilePath { get; set; } = Path.Combine("data", "zones.json");
}
