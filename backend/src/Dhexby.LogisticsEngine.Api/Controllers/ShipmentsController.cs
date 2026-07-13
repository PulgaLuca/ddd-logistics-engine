using Dhexby.LogisticsEngine.Api.Extensions;
using Dhexby.LogisticsEngine.Application.Shipments;
using Dhexby.LogisticsEngine.Application.Shipments.DispatchShipment;
using Dhexby.LogisticsEngine.Application.Shipments.GetShipmentById;
using Dhexby.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dhexby.LogisticsEngine.Api.Controllers;

/// <summary>
/// Questo Controller rappresenta il punto di ingresso delle richieste HTTP relative agli Shipment.
/// Il Controller NON contiene la logica di business:
/// riceve la richiesta, crea un Command/Query e lo invia a MediatR.
/// Sarà poi l'Application Layer ad eseguire il lavoro.
/// </summary>
[ApiController] // Indica che questa classe è un controller API.
[Route("api/shipments")] // URL base del controller.
public sealed class ShipmentsController : ControllerBase
{
    // MediatR è il "ponte" tra il Controller e l'Application Layer.
    // Il Controller non conosce Handler, Repository o Database.
    private readonly IMediator _sender;

    // Dependency Injection: ASP.NET Core crea automaticamente IMediator e lo passa al costruttore.
    public ShipmentsController(IMediator sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Crea un nuovo Shipment.
    /// POST /api/shipments
    /// </summary>
    [HttpPost]

    // Se tutto va bene restituisce HTTP 201 Created con il Guid creato.
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]

    // Se la richiesta non è valida restituisce HTTP 400.
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]

    // Se esiste già uno Shipment incompatibile restituisce HTTP 409.
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(CreateShipmentRequest request, CancellationToken cancellationToken)
    {
        // Il DTO ricevuto dal client viene trasformato in un Command. ToCommand() è un metodo di estensione.
        Result<Guid> result = await _sender.Send(request.ToCommand(), cancellationToken);

        // Se qualcosa è andato storto restituiamo un ProblemDetails con il codice HTTP corretto.
        if (result.IsFailure)
            return result.ToProblemResult();

        // Se tutto è andato bene:
        // - restituiamo HTTP 201 (Created)
        // - indichiamo dove recuperare la nuova risorsa (GetById)
        // - restituiamo il Guid creato.
        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    /// <summary>
    /// Recupera uno Shipment tramite il suo identificatore.
    /// GET /api/shipments/{id}
    /// </summary>
    [HttpGet("{id:guid}")]

    // Se trovato restituisce HTTP 200 con lo Shipment.
    [ProducesResponseType(typeof(ShipmentResponse), StatusCodes.Status200OK)]

    // Se non esiste restituisce HTTP 404.
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        // Creiamo una Query e la inviamo a MediatR.
        // Sarà l'handler corrispondente a recuperare i dati.
        Result<ShipmentResponse> result = await _sender.Send(new GetShipmentByIdQuery(id), cancellationToken);

        // Se il recupero è riuscito restituiamo HTTP 200.
        // Altrimenti convertiamo l'errore in ProblemDetails.
        return result.IsSuccess ? Ok(result.Value) : result.ToProblemResult();
    }

    /// <summary>
    /// Esegue il Dispatch dello Shipment.
    /// POST /api/shipments/{id}/dispatch
    /// </summary>
    [HttpPost("{id:guid}/dispatch")]

    // Operazione completata correttamente.
    [ProducesResponseType(StatusCodes.Status204NoContent)]

    // Shipment non trovato.
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]

    // Lo Shipment non può essere spedito (ad esempio è già stato spedito).
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Dispatch(Guid id, CancellationToken cancellationToken)
    {
        // Creiamo il Command e lo inviamo.
        // L'handler conterrà tutta la logica di business.
        Result result = await _sender.Send(new DispatchShipmentCommand(id), cancellationToken);

        // Se il Dispatch è riuscito restituiamo HTTP 204,
        // cioè operazione completata senza dati da restituire.
        return result.IsSuccess ? NoContent() : result.ToProblemResult();
    }
}
