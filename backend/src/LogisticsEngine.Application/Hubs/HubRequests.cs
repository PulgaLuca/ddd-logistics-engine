using LogisticsEngine.SharedKernel;
using MediatR;

namespace LogisticsEngine.Application.Hubs;

public sealed record GetHubById(Guid Id) : IRequest<Result<HubDto>>;
public sealed record GetAllHubs : IRequest<IReadOnlyList<HubDto>>;
public sealed record CreateHub(string? Name, string? Zone) : IRequest<Result<HubDto>>;
public sealed record UpdateHub(Guid Id, string? Name, string? Zone) : IRequest<Result>;
public sealed record DeleteHub(Guid Id) : IRequest<Result>;
