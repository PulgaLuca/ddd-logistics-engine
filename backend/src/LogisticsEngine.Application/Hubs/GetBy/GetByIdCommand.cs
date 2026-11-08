namespace LogisticsEngine.Application.Hubs.GetById;

/// <summary>Identifica l'Hub da recuperare tramite id.</summary>
public sealed record GetByIdCommand(Guid Id);
