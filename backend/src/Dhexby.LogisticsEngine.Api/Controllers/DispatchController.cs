using Dhexby.LogisticsEngine.Api.Extensions;
using Dhexby.LogisticsEngine.Application.Orders.Dispatching;
using Dhexby.LogisticsEngine.Application.Orders.Dispatching.GetDispatchHistory;
using Dhexby.LogisticsEngine.Application.Orders.Dispatching.RedoDispatch;
using Dhexby.LogisticsEngine.Application.Orders.Dispatching.UndoDispatch;
using Dhexby.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dhexby.LogisticsEngine.Api.Controllers;

/// <summary>
/// Adapter HTTP della cronologia di dispatch (struttura dati: Stack, undo/redo).
/// Ogni azione restituisce lo stato aggiornato delle pile, forma stabile per il frontend.
/// </summary>
[ApiController]
[Route("api/dispatch")]
public sealed class DispatchController : ControllerBase
{
    private readonly ISender _sender;

    public DispatchController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>Stato corrente delle pile undo/redo.</summary>
    [HttpGet("history")]
    [ProducesResponseType(typeof(DispatchHistoryResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHistory(CancellationToken cancellationToken)
    {
        Result<DispatchHistoryResponse> result = await _sender.Send(new GetDispatchHistoryQuery(), cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblemResult();
    }

    /// <summary>Annulla l'ultima assegnazione ordine -&gt; hub.</summary>
    [HttpPost("undo")]
    [ProducesResponseType(typeof(DispatchHistoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Undo(CancellationToken cancellationToken)
    {
        Result<DispatchHistoryResponse> result = await _sender.Send(new UndoDispatchCommand(), cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblemResult();
    }

    /// <summary>Ri-applica l'ultima assegnazione annullata.</summary>
    [HttpPost("redo")]
    [ProducesResponseType(typeof(DispatchHistoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Redo(CancellationToken cancellationToken)
    {
        Result<DispatchHistoryResponse> result = await _sender.Send(new RedoDispatchCommand(), cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblemResult();
    }
}
