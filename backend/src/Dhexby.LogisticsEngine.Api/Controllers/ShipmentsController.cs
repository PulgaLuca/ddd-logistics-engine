using Dhexby.LogisticsEngine.Api.Extensions;
using Dhexby.LogisticsEngine.Application.Shipments;
using Dhexby.LogisticsEngine.Application.Shipments.DispatchShipment;
using Dhexby.LogisticsEngine.Application.Shipments.GetShipmentById;
using Dhexby.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dhexby.LogisticsEngine.Api.Controllers;

/// <summary>
/// Adapter HTTP: dipende solo da MediatR (layer Application), mai dai
/// repository; ogni azione è una traduzione request -> command/query -> Result.
/// </summary>
[ApiController]
[Route("api/shipments")]
public sealed class ShipmentsController : ControllerBase
{
    private readonly ISender _sender;

    public ShipmentsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(CreateShipmentRequest request, CancellationToken cancellationToken)
    {
        Result<Guid> result = await _sender.Send(request.ToCommand(), cancellationToken);

        if (result.IsFailure)
        {
            return result.ToProblemResult();
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ShipmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        Result<ShipmentResponse> result = await _sender.Send(new GetShipmentByIdQuery(id), cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblemResult();
    }

    [HttpPost("{id:guid}/dispatch")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Dispatch(Guid id, CancellationToken cancellationToken)
    {
        Result result = await _sender.Send(new DispatchShipmentCommand(id), cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblemResult();
    }
}
