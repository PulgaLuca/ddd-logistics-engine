using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Routing.GetRoadNetwork;

/// <summary>Query: il grafo completo della rete (nodi hub + archi strade).</summary>
public sealed record GetRoadNetworkQuery : IRequest<Result<RoadNetworkResponse>>;
