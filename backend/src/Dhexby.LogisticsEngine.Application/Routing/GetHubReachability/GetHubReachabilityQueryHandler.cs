using Dhexby.LogisticsEngine.Domain.Routing;
using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Application.Routing.GetHubReachability;

/// <summary>Use case: costruisce la rete e ne analizza la raggiungibilità (BFS/DFS).</summary>
public sealed class GetHubReachabilityQueryHandler
    : IRequestHandler<GetHubReachabilityQuery, Result<ReachabilityResponse>>
{
    private readonly IRoadNetworkFactory _networkFactory;
    private readonly INetworkAnalysisService _analysisService;

    public GetHubReachabilityQueryHandler(
        IRoadNetworkFactory networkFactory,
        INetworkAnalysisService analysisService)
    {
        _networkFactory = networkFactory;
        _analysisService = analysisService;
    }

    public async Task<Result<ReachabilityResponse>> Handle(
        GetHubReachabilityQuery request,
        CancellationToken cancellationToken)
    {
        RoadNetworkContext context = await _networkFactory.BuildAsync(cancellationToken);

        Result<ReachabilityResult> analysis = _analysisService.CheckReachability(
            context.Network,
            request.OriginHubId,
            request.DestinationHubId);

        if (analysis.IsFailure)
        {
            return Result.Failure<ReachabilityResponse>(analysis.Error);
        }

        return ReachabilityResponse.From(
            request.OriginHubId,
            request.DestinationHubId,
            analysis.Value,
            context.Hubs);
    }
}
