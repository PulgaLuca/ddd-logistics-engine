using Dhexby.LogisticsEngine.Api.Extensions;
using Dhexby.LogisticsEngine.Application.Orders;
using Dhexby.LogisticsEngine.Application.Orders.AssignOrder;
using Dhexby.LogisticsEngine.Application.Orders.GetOrderById;
using Dhexby.LogisticsEngine.Application.Orders.PlaceOrder;
using Dhexby.LogisticsEngine.Application.Orders.Processing;
using Dhexby.LogisticsEngine.Application.Orders.Processing.GetExpressDispatchQueue;
using Dhexby.LogisticsEngine.Application.Orders.Processing.GetOrderProcessingQueue;
using Dhexby.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dhexby.LogisticsEngine.Api.Controllers;

/// <summary>
/// Adapter HTTP degli ordini: dipende solo dall'Application layer (MediatR),
/// mai dai repository.
/// </summary>
[ApiController]
[Route("api/orders")]
public sealed class OrdersController : ControllerBase
{
    private readonly ISender _sender;

    public OrdersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Place(PlaceOrderRequest request, CancellationToken cancellationToken)
    {
        Result<Guid> result = await _sender.Send(request.ToCommand(), cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value)
            : result.ToProblemResult();
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        Result<OrderResponse> result = await _sender.Send(new GetOrderByIdQuery(id), cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblemResult();
    }

    [HttpPost("{id:guid}/assign")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Assign(Guid id, AssignOrderRequest request, CancellationToken cancellationToken)
    {
        Result result = await _sender.Send(new AssignOrderCommand(id, request.HubId), cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblemResult();
    }

    /// <summary>Coda di processamento FIFO degli ordini in attesa (struttura dati: Queue).</summary>
    [HttpGet("processing-queue")]
    [ProducesResponseType(typeof(OrderQueueResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProcessingQueue(CancellationToken cancellationToken)
    {
        Result<OrderQueueResponse> result = await _sender.Send(
            new GetOrderProcessingQueueQuery(),
            cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblemResult();
    }

    /// <summary>Coda di dispatch a priorità per l'express delivery (struttura dati: Priority Queue).</summary>
    [HttpGet("express-queue")]
    [ProducesResponseType(typeof(OrderQueueResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetExpressQueue(CancellationToken cancellationToken)
    {
        Result<OrderQueueResponse> result = await _sender.Send(
            new GetExpressDispatchQueueQuery(),
            cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblemResult();
    }
}

/// <summary>Contratto HTTP di creazione ordine.</summary>
public sealed record PlaceOrderRequest(string OrderNumber, string CustomerName, string Priority)
{
    public PlaceOrderCommand ToCommand() => new(OrderNumber, CustomerName, Priority);
}

/// <summary>Contratto HTTP di assegnazione ordine a un hub.</summary>
public sealed record AssignOrderRequest(Guid HubId);
