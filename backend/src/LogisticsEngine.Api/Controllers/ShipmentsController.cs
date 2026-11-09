using LogisticsEngine.Api.Contracts;
using LogisticsEngine.Api;
using LogisticsEngine.Application.Shipments;
using LogisticsEngine.Application.Shipments.CreateShipment;
using LogisticsEngine.Application.Shipments.Delete;
using LogisticsEngine.Application.Shipments.GetAll;
using LogisticsEngine.Application.Shipments.GetById;
using LogisticsEngine.Application.Shipments.UpdateStatus;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LogisticsEngine.Api.Controllers;

/// <summary>Espone endpoint HTTP Shipment e inoltra le operazioni a request MediatR.</summary>
[Route(ApiRouteConstants.VersionOnePrefix + "/shipments")]
public sealed class ShipmentsController(ISender sender) : ApiControllerBase
{
    /// <summary>Restituisce tutte le spedizioni.</summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ShipmentDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ShipmentDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await sender.Send(new GetAllShipmentsQuery(), cancellationToken));
    }
        

    /// <summary>Restituisce una spedizione identificata dal suo ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType<ShipmentDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetShipmentByIdQuery(id), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : ToProblem(result.Error);
    }

    /// <summary>Crea una spedizione in stato Created.</summary>
    [HttpPost]
    [ProducesResponseType<ShipmentDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateShipmentRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateShipmentCommand(request.TrackingNumber), cancellationToken);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value)
            : ToProblem(result.Error);
    }

    /// <summary>Richiede una transizione valida dello stato della spedizione.</summary>
    [HttpPut("{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        [FromBody] UpdateShipmentStatusRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new UpdateShipmentStatusCommand(id, request.Status), cancellationToken);
        return result.IsSuccess ? NoContent() : ToProblem(result.Error);
    }

    /// <summary>Elimina una spedizione.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteShipmentCommand(id), cancellationToken);
        return result.IsSuccess ? NoContent() : ToProblem(result.Error);
    }
}
