namespace Dhexby.LogisticsEngine.Domain.Shipments;

/// <summary>
/// Ciclo di vita della spedizione: Pending -> InTransit -> Delivered.
/// Cancelled è raggiungibile solo da Pending.
/// </summary>
public enum ShipmentStatus
{
    Pending = 0,
    InTransit = 1,
    Delivered = 2,
    Cancelled = 3
}
