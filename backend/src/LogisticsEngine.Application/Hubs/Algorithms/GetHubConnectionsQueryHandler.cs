using LogisticsEngine.Application.Hubs;
using LogisticsEngine.Domain.Hubs;
using MediatR;

namespace LogisticsEngine.Application.Hubs.Algorithms;

public sealed class GetHubConnectionsQueryHandler(IHubConnectionRepository repository)
    : IRequestHandler<GetHubConnectionsQuery, IReadOnlyList<HubConnectionDto>>
{
    public Task<IReadOnlyList<HubConnectionDto>> Handle(GetHubConnectionsQuery query, CancellationToken cancellationToken)
    {
        IReadOnlyList<HubConnectionDto> connections = repository.GetAll()
            .Select(connection => new HubConnectionDto(connection.FirstHubId, connection.SecondHubId, connection.Distance))
            .ToArray();
        return Task.FromResult(connections);
    }
}
