namespace LogisticsEngine.Application.Hubs.GetBy;

/// <summary>Identifica l'Hub da recuperare tramite id.</summary>
public sealed record GetByNameCommand(string? name);
