using LogisticsEngine.SharedKernel;
using MediatR;

namespace LogisticsEngine.Application.Hubs.DeleteHub;

/// <summary>Identifica l'Hub da eliminare.</summary>
public sealed record DeleteHubCommand(Guid Id) : IRequest<Result>;
