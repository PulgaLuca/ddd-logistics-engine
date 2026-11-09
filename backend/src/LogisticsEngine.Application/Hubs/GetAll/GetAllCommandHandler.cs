using LogisticsEngine.Application.Hubs;
using LogisticsEngine.Domain.Hubs;
using MediatR;

namespace LogisticsEngine.Application.Hubs.GetAll;

/// <summary>Carica gli Hub tramite il repository e li espone come DTO applicativi.</summary>
public sealed class GetAllCommandHandler(IHubRepository repository) : IRequestHandler<GetAllHubsQuery, IReadOnlyList<HubDto>>
{
    public Task<IReadOnlyList<HubDto>> Handle(GetAllHubsQuery query, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<HubDto>>(repository.GetAll().Select(HubDto.From).ToArray());
}
