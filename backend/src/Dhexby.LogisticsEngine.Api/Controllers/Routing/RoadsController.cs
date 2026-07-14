using Dhexby.LogisticsEngine.Api.Authorization;
using Dhexby.LogisticsEngine.Api.Extensions;
using Dhexby.LogisticsEngine.Application.Routing;
using Dhexby.LogisticsEngine.Application.Routing.AddRoad;
using Dhexby.LogisticsEngine.Application.Routing.GetRoadNetwork;
using Dhexby.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dhexby.LogisticsEngine.Api.Controllers;

/// <summary>
/// Adapter HTTP delle strade (archi del grafo) e del grafo della rete.
/// </summary>
[ApiController]
[Route("api/roads")]
[Authorize]
public sealed class RoadsController : ControllerBase
{
    private readonly ISender _sender;

    public RoadsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>Aggiunge una strada bidirezionale tra due hub.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [Authorize(Roles = Roles.ManageData)]
    public async Task<IActionResult> Add(AddRoadRequest request, CancellationToken cancellationToken)
    {
        Result<Guid> result = await _sender.Send(request.ToCommand(), cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetNetwork), routeValues: null, result.Value)
            : result.ToProblemResult();
    }

    /// <summary>Il grafo completo della rete (nodi hub + archi strade).</summary>
    [HttpGet("network")]
    [ProducesResponseType(typeof(RoadNetworkResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNetwork(CancellationToken cancellationToken)
    {
        Result<RoadNetworkResponse> result = await _sender.Send(new GetRoadNetworkQuery(), cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblemResult();
    }
}

/// <summary>Contratto HTTP di aggiunta strada.</summary>
public sealed record AddRoadRequest(
    Guid FromHubId,
    Guid ToHubId,
    double DistanceInKilometers,
    double TrafficFactor)
{
    public AddRoadCommand ToCommand() => new(FromHubId, ToHubId, DistanceInKilometers, TrafficFactor);
}
