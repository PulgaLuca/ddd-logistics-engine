using Dhexby.SharedKernel;
using Microsoft.AspNetCore.Mvc;

namespace Dhexby.LogisticsEngine.Api.Extensions;

/// <summary>
/// Traduzione unica (DRY) da Result/Error del dominio a risposta HTTP
/// ProblemDetails, così i controller restano privi di logica.
/// </summary>
public static class ResultExtensions
{
    public static IActionResult ToProblemResult(this Result result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("Cannot convert a success result to a problem response.");
        }

        int statusCode = result.Error.Type switch
        {
            ErrorType.Validation or ErrorType.Failure => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError,
        };

        return new ObjectResult(new ProblemDetails
        {
            Status = statusCode,
            Title = result.Error.Code,
            Detail = result.Error.Description,
        })
        {
            StatusCode = statusCode,
        };
    }
}
