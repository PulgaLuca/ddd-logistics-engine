
using LogisticsEngine.Api.Contracts;
using LogisticsEngine.Application.Hubs;
using LogisticsEngine.Application.Hubs.Algorithms;
using LogisticsEngine.Application.Hubs.DeleteHub;
using LogisticsEngine.Application.Hubs.GetAll;
using LogisticsEngine.Application.Hubs.GetBy;
using LogisticsEngine.Application.Hubs.GetById;
using LogisticsEngine.Application.Hubs.RegisterHub;
using LogisticsEngine.Application.Hubs.UpdateHub;
using MediatR;
using Microsoft.AspNetCore.Mvc;

// Namespace: definisce il contesto logico a cui appartiene la classe.
namespace LogisticsEngine.Api.Controllers;

/// <summary>
/// Controller ASP.NET Core che espone gli endpoint HTTP relativi agli Hub.
/// Ogni operazione viene delegata a MediatR, evitando di inserire
/// la logica applicativa direttamente nel controller.
/// </summary>

// Route: definisce il percorso base comune a tutti gli endpoint.
// ApiRouteConstants.VersionOnePrefix contiene il prefisso della versione API.
// /hubs viene aggiunto a tale prefisso.
[Route(ApiRouteConstants.VersionOnePrefix + "/hubs")]

// sealed: impedisce che questa classe venga ereditata.
// ISender: interfaccia MediatR utilizzata per inviare query e command.
// (ISender sender): primary constructor di C# 12.
// Il parametro sender viene dichiarato direttamente nella firma della classe
// ed è accessibile dai metodi di istanza.
public sealed class HubsController(ISender sender) : ApiControllerBase
{
    /// <summary>Restituisce tutti gli Hub.</summary>

    // HttpGet: associa il metodo alle richieste HTTP GET.
    // Quando si visita /api/v1/hubs viene eseguito questo metodo.
    //
    // ProducesResponseType: documenta il tipo di risposta HTTP.
    // Status200OK indica che la richiesta è stata completata correttamente.
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<HubDto>>(StatusCodes.Status200OK)]

    // async: permette di utilizzare await all'interno del metodo.
    // Task<ActionResult<T>>: rappresenta un'operazione asincrona
    // che restituirà una risposta HTTP contenente una lista di HubDto.
    //
    // CancellationToken: consente di interrompere l'operazione
    // se la richiesta HTTP viene annullata.
    public async Task<ActionResult<IReadOnlyList<HubDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        // await: attende il completamento dell'operazione asincrona
        // senza bloccare il thread durante l'attesa.
        //
        // sender.Send: invia una Query a MediatR.
        // GetAllHubsQuery rappresenta una richiesta di lettura.
        //
        // Ok(): restituisce HTTP 200 con i dati ottenuti.
        return Ok(await sender.Send(
            new GetAllHubsQuery(), cancellationToken));
    }

    /// <summary>Restituisce un Hub identificato dal suo ID.</summary>

    // {id:guid}: parametro di route.
    // guid è un vincolo che accetta solamente valori compatibili
    // con il formato GUID.
    [HttpGet("{id:guid}")]
    [ProducesResponseType<HubDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]

    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        // new: crea una nuova istanza della Query.
        // La Query contiene l'identificativo dell'Hub da cercare.
        var result = await sender.Send(
            new GetHubByIdQuery(id), cancellationToken);

        // Operatore ternario:
        // condizione ? valore_se_vero : valore_se_falso.
        //
        // IsSuccess: verifica se l'operazione è riuscita.
        // Value: contiene il risultato.
        // ToProblem(): converte l'errore in una risposta HTTP ProblemDetails.
        //
        // Ok(): restituisce HTTP 200.
        return result.IsSuccess
            ? Ok(result.Value)
            : ToProblem(result.Error);
    }

    /// <summary>Cerca un Hub tramite nome.</summary>

    // {name}: parametro dinamico inserito nell'URL.
    [HttpGet("by-name/{name}")]
    [ProducesResponseType<HubDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]

    public async Task<IActionResult> GetByName(
        string name,
        CancellationToken cancellationToken)
    {
        // Invio della Query che cerca un Hub in base al nome.
        var result = await sender.Send(
            new GetHubByNameQuery(name), cancellationToken);

        // Se la ricerca ha successo restituisce il DTO,
        // altrimenti restituisce il dettaglio dell'errore.
        return result.IsSuccess
            ? Ok(result.Value)
            : ToProblem(result.Error);
    }

    /// <summary>Crea un Hub e restituisce il link alla risorsa creata.</summary>

    // HttpPost: associa il metodo alle richieste HTTP POST.
    // POST viene utilizzato per creare nuove risorse.
    [HttpPost]
    [ProducesResponseType<HubDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]

    public async Task<IActionResult> Create(
        [FromBody] CreateHubRequest request,
        CancellationToken cancellationToken)
    {
        // FromBody: indica che il parametro request deve essere
        // costruito a partire dal corpo della richiesta HTTP.
        //
        // Il model binding di ASP.NET Core converte il JSON
        // ricevuto in un oggetto CreateHubRequest.
        //
        // RegisterHubCommand: Command che richiede la creazione di un Hub.
        var result = await sender.Send(
            new RegisterHubCommand(request.Name, request.Zone),
            cancellationToken);

        // Operatore ternario.
        //
        // CreatedAtAction:
        // restituisce HTTP 201 Created.
        // Genera anche l'URL della risorsa appena creata,
        // utilizzando l'endpoint GetById e il suo parametro id.
        //
        // Se la creazione fallisce, restituisce il relativo errore.
        return result.IsSuccess
            ? CreatedAtAction(
                nameof(GetById),
                new { id = result.Value.Id },
                result.Value)
            : ToProblem(result.Error);
    }

    /// <summary>Aggiorna nome e zona di un Hub esistente.</summary>

    // HttpPut: associa il metodo alle richieste HTTP PUT.
    // PUT viene utilizzato per aggiornare una risorsa.
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]

    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateHubRequest request,
        CancellationToken cancellationToken)
    {
        // Il Command contiene:
        // - ID dell'Hub da aggiornare;
        // - nuovo nome;
        // - nuova zona.
        var result = await sender.Send(
            new UpdateHubCommand(id, request.Name, request.Zone),
            cancellationToken);

        // NoContent(): restituisce HTTP 204.
        // Significa che l'operazione è riuscita
        // ma non viene restituito alcun contenuto nel body.
        return result.IsSuccess
            ? NoContent()
            : ToProblem(result.Error);
    }

    /// <summary>Elimina un Hub.</summary>

    // HttpDelete: associa il metodo alle richieste HTTP DELETE.
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]

    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        // Invio del Command che richiede l'eliminazione dell'Hub.
        var result = await sender.Send(
            new DeleteHubCommand(id), cancellationToken);

        // Se l'eliminazione riesce restituisce HTTP 204.
        // Altrimenti restituisce l'errore.
        return result.IsSuccess
            ? NoContent()
            : ToProblem(result.Error);
    }

    /// <summary>Ordina gli Hub per ID, nome o zona.</summary>

    // algorithms/sort: endpoint dedicato agli algoritmi.
    [HttpGet("algorithms/sort")]

    // ActionResult<IReadOnlyList<HubDto>>:
    // risposta HTTP che contiene una lista di DTO in sola lettura.
    //
    // FromQuery: il parametro viene recuperato dalla query string.
    //
    // HubSortProperty.Id: valore predefinito del parametro.
    // Se il client non specifica property, viene utilizzato Id.
    //
    // = default: assegna un valore predefinito al CancellationToken.
    //
    // =>: expression-bodied member.
    // Permette di scrivere un metodo con una sola espressione
    // senza utilizzare le parentesi graffe.
    public async Task<ActionResult<IReadOnlyList<HubDto>>> Sort(
        [FromQuery] HubSortProperty property = HubSortProperty.Id,
        CancellationToken cancellationToken = default) =>
        Ok(await sender.Send(
            new GetSortedHubsQuery(property), cancellationToken));

    /// <summary>Raggruppa gli Hub per zona.</summary>

    [HttpGet("algorithms/groups-by-zone")]

    // Il metodo utilizza nuovamente la sintassi expression-bodied (=>).
    // GetHubsGroupedByZoneQuery richiede il raggruppamento degli Hub
    // in base alla loro zona.
    public async Task<ActionResult<IReadOnlyList<HubGroupDto>>> GroupByZone(
        CancellationToken cancellationToken) =>
        Ok(await sender.Send(
            new GetHubsGroupedByZoneQuery(), cancellationToken));

    /// <summary>Dimostra queue, stack, hashmap e set sui dati Hub.</summary>

    [HttpGet("algorithms/collections")]

    // HubCollectionsDto rappresenta il tipo di dato restituito.
    // L'endpoint mostra i risultati delle operazioni sulle collezioni.
    public async Task<ActionResult<HubCollectionsDto>> Collections(
        CancellationToken cancellationToken) =>
        Ok(await sender.Send(
            new GetHubCollectionsQuery(), cancellationToken));

    /// <summary>Restituisce la visita in-order dell'albero binario per ID.</summary>

    [HttpGet("algorithms/binary-tree")]

    // Restituisce una lista di Hub secondo la visita in-order
    // di un albero binario.
    public async Task<ActionResult<IReadOnlyList<HubDto>>> BinaryTree(
        CancellationToken cancellationToken) =>
        Ok(await sender.Send(
            new GetHubTreeOrderQuery(), cancellationToken));

    /// <summary>Cerca Hub il cui nome inizia con il prefisso indicato.</summary>

    [HttpGet("algorithms/trie")]

    public async Task<ActionResult<IReadOnlyList<HubDto>>> SearchPrefix(
        // string?: tipo string nullable.
        // Il simbolo ? indica che il parametro può essere null.
        //
        // FromQuery: il valore viene letto dalla query string.
        // Esempio: /hubs/algorithms/trie?prefix=Rom
        [FromQuery] string? prefix,
        CancellationToken cancellationToken) =>

        Ok(await sender.Send(
            new SearchHubsByPrefixQuery(prefix), cancellationToken));

    /// <summary>Aggiunge un collegamento bidirezionale pesato tra due Hub.</summary>

    // POST /hubs/connections
    [HttpPost("connections")]
    [ProducesResponseType<HubConnectionDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]

    public async Task<IActionResult> Connect(
        [FromBody] CreateHubConnectionRequest request,
        CancellationToken cancellationToken)
    {
        // Il Command contiene gli ID dei due Hub e la distanza
        // che rappresenta il peso dell'arco nel grafo.
        var result = await sender.Send(
            new ConnectHubsCommand(
                request.FirstHubId,
                request.SecondHubId,
                request.Distance),
            cancellationToken);

        // StatusCode(201, result.Value):
        // restituisce HTTP 201 Created con il DTO del collegamento.
        //
        // A differenza di CreatedAtAction, qui non viene generato
        // automaticamente un URL verso un altro endpoint.
        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, result.Value)
            : ToProblem(result.Error);
    }

    /// <summary>Elenca i collegamenti salvati nel grafo.</summary>

    [HttpGet("connections")]

    public async Task<ActionResult<IReadOnlyList<HubConnectionDto>>> GetConnections(
        CancellationToken cancellationToken) =>
        Ok(await sender.Send(
            new GetHubConnectionsQuery(), cancellationToken));

    /// <summary>Calcola il percorso di distanza minima tra due Hub con Dijkstra.</summary>

    [HttpGet("algorithms/shortest-path")]

    public async Task<IActionResult> ShortestPath(
        // I due parametri vengono recuperati dalla query string.
        // Esempio:
        // ?originId=GUID1&destinationId=GUID2
        [FromQuery] Guid originId,
        [FromQuery] Guid destinationId,
        CancellationToken cancellationToken)
    {
        // Invio della Query che calcola il percorso minimo
        // utilizzando l'algoritmo di Dijkstra.
        var result = await sender.Send(
            new FindShortestHubPathQuery(originId, destinationId),
            cancellationToken);

        // Restituisce il risultato oppure un errore HTTP.
        return result.IsSuccess
            ? Ok(result.Value)
            : ToProblem(result.Error);
    }

    /// <summary>Visita il grafo con BFS oppure DFS a partire dall'Hub indicato.</summary>

    [HttpGet("algorithms/traversal")]

    public async Task<IActionResult> Traverse(
        [FromQuery] Guid originId,

        // HubGraphTraversal probabilmente è un enum.
        // Consente di scegliere il tipo di visita del grafo:
        // BFS (Breadth-First Search) oppure DFS (Depth-First Search).
        [FromQuery] HubGraphTraversal traversal,

        CancellationToken cancellationToken)
    {
        // La Query riceve il nodo iniziale e l'algoritmo da utilizzare.
        var result = await sender.Send(
            new GetHubTraversalQuery(originId, traversal),
            cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ToProblem(result.Error);
    }

    /// <summary>Trova cammini semplici con backtracking, fino al limite specificato.</summary>

    [HttpGet("algorithms/simple-paths")]

    public async Task<IActionResult> SimplePaths(
        [FromQuery] Guid originId,
        [FromQuery] Guid destinationId,

        // maximumPaths indica il numero massimo di percorsi
        // semplici che si desidera ottenere.
        [FromQuery] int maximumPaths,

        CancellationToken cancellationToken)
    {
        // Invio della Query che utilizza il backtracking
        // per individuare percorsi senza ripetere nodi.
        var result = await sender.Send(
            new FindSimpleHubPathsQuery(
                originId,
                destinationId,
                maximumPaths),
            cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : ToProblem(result.Error);
    }
}
