using LogisticsEngine.Application.Hubs;
using LogisticsEngine.SharedKernel;
using MediatR;

namespace LogisticsEngine.Application.Hubs.Algorithms;

public enum HubGraphTraversal
{
    BreadthFirst,
    DepthFirst
}

public sealed record GetHubTraversalQuery(Guid OriginId, HubGraphTraversal Traversal)
    : IRequest<Result<IReadOnlyList<HubDto>>>;
