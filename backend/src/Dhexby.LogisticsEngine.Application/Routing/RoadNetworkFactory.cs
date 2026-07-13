using Dhexby.LogisticsEngine.Domain.Hubs;
using Dhexby.LogisticsEngine.Domain.Routing;

namespace Dhexby.LogisticsEngine.Application.Routing;

/// <summary>Rete stradale già costruita insieme alla lookup degli hub per nome/coordinate.</summary>
public sealed class RoadNetworkContext
{
    public RoadNetworkContext(RoadNetwork network, IReadOnlyDictionary<Guid, Hub> hubs)
    {
        Network = network;
        Hubs = hubs;
    }

    public RoadNetwork Network { get; }

    public IReadOnlyDictionary<Guid, Hub> Hubs { get; }
}

/// <summary>
/// Carica hub e strade dai repository e assembla il grafo (DRY: usato da tutti
/// gli use case di routing e analisi di rete).
/// </summary>
public interface IRoadNetworkFactory
{
    Task<RoadNetworkContext> BuildAsync(CancellationToken cancellationToken);
}

public sealed class RoadNetworkFactory : IRoadNetworkFactory
{
    private readonly IHubRepository _hubRepository;
    private readonly IRoadRepository _roadRepository;

    public RoadNetworkFactory(IHubRepository hubRepository, IRoadRepository roadRepository)
    {
        _hubRepository = hubRepository;
        _roadRepository = roadRepository;
    }

    public async Task<RoadNetworkContext> BuildAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Hub> hubs = await _hubRepository.GetAllAsync(cancellationToken);
        IReadOnlyList<Road> roads = await _roadRepository.GetAllAsync(cancellationToken);

        RoadNetwork network = RoadNetwork.Build(hubs, roads);
        Dictionary<Guid, Hub> lookup = hubs.ToDictionary(hub => hub.Id);

        return new RoadNetworkContext(network, lookup);
    }
}
