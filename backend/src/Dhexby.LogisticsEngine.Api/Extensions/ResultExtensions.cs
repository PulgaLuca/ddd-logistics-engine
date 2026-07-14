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

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = result.Error.Code,
            Detail = result.Error.Description,
        };

        // Se è un errore aggregato di validazione, esponiamo il dettaglio
        // per proprietà così il client vede tutte le violazioni insieme.
        if (result.Error is ValidationError validationError)
        {
            problemDetails.Extensions["errors"] = validationError.Errors
                .Select(error => new { error.Code, error.Description })
                .ToArray();
        }

        return new ObjectResult(problemDetails)
        {
            StatusCode = statusCode,
        };
    }
}
