namespace Dhexby.LogisticsEngine.Application.Routing;

/// <summary>Rappresentazione del grafo (nodi + archi) per la visualizzazione.</summary>
public sealed record RoadNetworkResponse(
    IReadOnlyList<RoadNetworkNodeResponse> Nodes,
    IReadOnlyList<RoadNetworkEdgeResponse> Edges);

public sealed record RoadNetworkNodeResponse(
    Guid HubId,
    string HubCode,
    string Name,
    double Latitude,
    double Longitude);

public sealed record RoadNetworkEdgeResponse(
    Guid FromHubId,
    Guid ToHubId,
    double DistanceInKilometers,
    double TrafficFactor);
