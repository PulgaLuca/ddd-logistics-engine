using LogisticsEngine.Application.Hubs;
using MediatR;

namespace LogisticsEngine.Application.Hubs.GetAll;

/// <summary>Richiede l'elenco degli Hub disponibili.</summary>
public sealed record GetAllHubsQuery : IRequest<IReadOnlyList<HubDto>>;
