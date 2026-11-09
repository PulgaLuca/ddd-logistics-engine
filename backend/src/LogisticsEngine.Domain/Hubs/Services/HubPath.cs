namespace LogisticsEngine.Domain.Hubs.Services;

/// <summary>Percorso tra Hub e relativo costo totale.</summary>
public sealed record HubPath(IReadOnlyList<Guid> HubIds, decimal TotalDistance);
