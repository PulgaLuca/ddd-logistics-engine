using Dhexby.LogisticsEngine.Api.Extensions;
using Dhexby.LogisticsEngine.Application.Routing;
using Dhexby.LogisticsEngine.Application.Routing.GetHubReachability;
using Dhexby.LogisticsEngine.Application.Routing.GetShortestRoute;
using Dhexby.LogisticsEngine.Application.Routing.GetSmartRoute;
using Dhexby.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dhexby.LogisticsEngine.Api.Controllers;

/// <summary>
/// Adapter HTTP del calcolo percorsi e dell'analisi di rete. Tutte le rotte
/// restituiscono lo stesso tipo di risultato al variare della strategia.
/// </summary>
[ApiController]
[Route("api/routes")]
[Authorize]
public sealed class RoutesController : ControllerBase
{
    private readonly ISender _sender;

    public RoutesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>Percorso più corto per distanza (Dijkstra).</summary>
    [HttpGet("shortest")]
    [ProducesResponseType(typeof(RouteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetShortest(
        [FromQuery] Guid originHubId,
        [FromQuery] Guid destinationHubId,
        CancellationToken cancellationToken)
    {
        Result<RouteResponse> result = await _sender.Send(
            new GetShortestRouteQuery(originHubId, destinationHubId),
            cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblemResult();
    }

    /// <summary>Percorso "smart" traffico-consapevole (A*).</summary>
    [HttpGet("smart")]
    [ProducesResponseType(typeof(RouteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSmart(
        [FromQuery] Guid originHubId,
        [FromQuery] Guid destinationHubId,
        CancellationToken cancellationToken)
    {
        Result<RouteResponse> result = await _sender.Send(
            new GetSmartRouteQuery(originHubId, destinationHubId),
            cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblemResult();
    }

    /// <summary>Raggiungibilità tra due hub (BFS/DFS): "posso raggiungere questo hub?".</summary>
    [HttpGet("reachability")]
    [ProducesResponseType(typeof(ReachabilityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReachability(
        [FromQuery] Guid originHubId,
        [FromQuery] Guid destinationHubId,
        CancellationToken cancellationToken)
    {
        Result<ReachabilityResponse> result = await _sender.Send(
            new GetHubReachabilityQuery(originHubId, destinationHubId),
            cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblemResult();
    }
}
