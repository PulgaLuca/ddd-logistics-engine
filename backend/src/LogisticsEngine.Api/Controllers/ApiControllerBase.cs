using LogisticsEngine.SharedKernel;
using Microsoft.AspNetCore.Mvc;

namespace LogisticsEngine.Api.Controllers;

/// <summary>Converte gli errori applicativi Result in risposte HTTP ProblemDetails.</summary>
[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult ToProblem(Error error)
    {
        int statusCode = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };

        return Problem(statusCode: statusCode, title: error.Code, detail: error.Description);
    }
}
