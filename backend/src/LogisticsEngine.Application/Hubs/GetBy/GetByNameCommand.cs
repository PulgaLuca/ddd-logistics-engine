using LogisticsEngine.Application.Hubs;
using LogisticsEngine.SharedKernel;
using MediatR;

namespace LogisticsEngine.Application.Hubs.GetBy;

/// <summary>Identifica l'Hub da recuperare tramite id.</summary>
public sealed record GetHubByNameQuery(string? Name) : IRequest<Result<HubDto>>;
