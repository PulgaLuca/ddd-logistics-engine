namespace Dhexby.LogisticsEngine.Domain.Routing;

/// <summary>
/// Risultato di un calcolo di percorso: la sequenza ordinata di hub attraversati
/// e il costo totale (nell'unità del calcolatore che l'ha prodotto).
/// </summary>
public sealed record RoutePath(IReadOnlyList<Guid> HubIds, double TotalCost);
