using LogisticsEngine.Application.Hubs;
using LogisticsEngine.SharedKernel;
using MediatR;

namespace LogisticsEngine.Application.Hubs.GetById;

/// <summary>Identifica l'Hub da recuperare tramite id.</summary>
public sealed record GetHubByIdQuery(Guid Id) : IRequest<Result<HubDto>>;
