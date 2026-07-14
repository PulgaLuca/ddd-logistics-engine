using Dhexby.LogisticsEngine.Domain.Hubs;
using Dhexby.LogisticsEngine.Domain.Routing;
using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Routing.GetRoadNetwork;

/// <summary>Use case di lettura: proietta l'intero grafo della rete.</summary>
public sealed class GetRoadNetworkQueryHandler : IRequestHandler<GetRoadNetworkQuery, Result<RoadNetworkResponse>>
{
    private readonly IHubRepository _hubRepository;
    private readonly IRoadRepository _roadRepository;

    public GetRoadNetworkQueryHandler(IHubRepository hubRepository, IRoadRepository roadRepository)
    {
        _hubRepository = hubRepository;
        _roadRepository = roadRepository;
    }

    public async Task<Result<RoadNetworkResponse>> Handle(
        GetRoadNetworkQuery request,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<Hub> hubs = await _hubRepository.GetAllAsync(cancellationToken);
        IReadOnlyList<Road> roads = await _roadRepository.GetAllAsync(cancellationToken);

        var nodes = hubs
            .Select(hub => new RoadNetworkNodeResponse(
                hub.Id,
                hub.Code.Value,
                hub.Name,
                hub.Coordinate.Latitude,
                hub.Coordinate.Longitude))
            .ToArray();

        var edges = roads
            .Select(road => new RoadNetworkEdgeResponse(
                road.FromHubId,
                road.ToHubId,
                road.DistanceInKilometers,
                road.TrafficFactor))
            .ToArray();

        return new RoadNetworkResponse(nodes, edges);
    }
}
