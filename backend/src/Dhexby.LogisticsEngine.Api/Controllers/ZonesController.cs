using Dhexby.LogisticsEngine.Api.Extensions;
using Dhexby.LogisticsEngine.Application.Zones;
using Dhexby.LogisticsEngine.Application.Zones.AddStreetToZone;
using Dhexby.LogisticsEngine.Application.Zones.CreateZone;
using Dhexby.LogisticsEngine.Application.Zones.GetZoneById;
using Dhexby.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dhexby.LogisticsEngine.Api.Controllers;

/// <summary>Adapter HTTP delle zone: dipende solo dall'Application layer (MediatR).</summary>
[ApiController]
[Route("api/zones")]
public sealed class ZonesController : ControllerBase
{
    private readonly ISender _sender;

    public ZonesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(CreateZoneRequest request, CancellationToken cancellationToken)
    {
        Result<Guid> result = await _sender.Send(request.ToCommand(), cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value)
            : result.ToProblemResult();
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ZoneResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        Result<ZoneResponse> result = await _sender.Send(new GetZoneByIdQuery(id), cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblemResult();
    }

    [HttpPost("{id:guid}/streets")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddStreet(Guid id, AddStreetRequest request, CancellationToken cancellationToken)
    {
        Result result = await _sender.Send(
            new AddStreetToZoneCommand(id, request.StreetName, request.PostalCode),
            cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblemResult();
    }
}

/// <summary>Contratto HTTP di creazione zona (ParentZoneId opzionale per le sottozone).</summary>
public sealed record CreateZoneRequest(string Code, string Name, Guid? ParentZoneId)
{
    public CreateZoneCommand ToCommand() => new(Code, Name, ParentZoneId);
}

/// <summary>Contratto HTTP di aggiunta via a una zona.</summary>
public sealed record AddStreetRequest(string StreetName, string PostalCode);
