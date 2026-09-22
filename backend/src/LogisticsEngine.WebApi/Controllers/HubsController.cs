using LogisticsEngine.Application.Hubs;
using LogisticsEngine.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LogisticsEngine.WebApi.Controllers;

[ApiController]
[Route("api/hubs")]
public sealed class HubsController(ISender sender) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        Result<HubDto> result = await sender.Send(new GetHubById(id), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : ToProblem(result.Error);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await sender.Send(new GetAllHubs(), cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create(HubInput input, CancellationToken cancellationToken)
    {
        Result<HubDto> result = await sender.Send(new CreateHub(input.Name, input.Zone), cancellationToken);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value)
            : ToProblem(result.Error);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, HubInput input, CancellationToken cancellationToken)
    {
        Result result = await sender.Send(new UpdateHub(id, input.Name, input.Zone), cancellationToken);
        return result.IsSuccess ? NoContent() : ToProblem(result.Error);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        Result result = await sender.Send(new DeleteHub(id), cancellationToken);
        return result.IsSuccess ? NoContent() : ToProblem(result.Error);
    }

    private ObjectResult ToProblem(Error error) =>
        Problem(statusCode: error.Type == ErrorType.NotFound ? 404 : 400,
            title: error.Code, detail: error.Description);

    public sealed record HubInput(string? Name, string? Zone);
}
