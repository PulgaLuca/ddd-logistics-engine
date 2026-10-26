using Dhexby.LogisticsEngine.Api.Authorization;
using Dhexby.LogisticsEngine.Api.Controllers.Hubs;
using Dhexby.LogisticsEngine.Api.Extensions;
using Dhexby.LogisticsEngine.Application.Hubs;
using Dhexby.LogisticsEngine.Application.Hubs.GetHubById;
using Dhexby.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dhexby.LogisticsEngine.Api.Controllers;

/// <summary>Adapter HTTP degli hub: dipende solo dall'Application layer (MediatR).</summary>
[ApiController]
[Route("api/hubs")]
[Authorize]
public sealed class HubsController : ControllerBase
{
    private readonly IMediator _sender;

    public HubsController(IMediator sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [Authorize(Roles = Roles.ManageData)]
    public async Task<IActionResult> Register(RegisterHubRequest request, CancellationToken cancellationToken)
    {
        Result<Guid> result = await _sender.Send(request.ToCommand(), cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value)
            : result.ToProblemResult();
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(HubResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        Result<HubResponse> result = await _sender.Send(new GetHubByIdQuery(id), cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblemResult();
    }
}
