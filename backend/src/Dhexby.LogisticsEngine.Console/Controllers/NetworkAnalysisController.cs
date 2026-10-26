using Dhexby.LogisticsEngine.Application.Routing;
using Dhexby.LogisticsEngine.Domain.Hubs;
using Dhexby.LogisticsEngine.Domain.Routing;

namespace Dhexby.LogisticsEngine.Console.Controllers;

/// <summary>Analizza la rete persistita e mostra i percorsi BFS e le visite BFS/DFS.</summary>
public sealed class NetworkAnalysisController
{
    private readonly IRoadNetworkFactory _networkFactory;
    private readonly INetworkAnalysisService _analysisService;

    public NetworkAnalysisController(
        IRoadNetworkFactory networkFactory,
        INetworkAnalysisService analysisService)
    {
        _networkFactory = networkFactory;
        _analysisService = analysisService;
    }

    public async Task ShowDemoAsync(CancellationToken cancellationToken = default)
    {
        RoadNetworkContext context = await _networkFactory.BuildAsync(cancellationToken);
        if (context.Hubs.Count == 0)
        {
            System.Console.WriteLine("Non ci sono hub. Registra almeno due hub e collega le strade prima dell'analisi.");
            return;
        }

        System.Console.WriteLine("Hub disponibili:");
        foreach (Hub hub in context.Hubs.Values.OrderBy(hub => hub.Code.Value))
        {
            System.Console.WriteLine($"{hub.Id} | {hub.Code.Value}");
        }

        System.Console.Write("ID hub di partenza: ");
        if (!Guid.TryParse(System.Console.ReadLine(), out Guid originId) || !context.Hubs.ContainsKey(originId))
        {
            System.Console.WriteLine("Hub di partenza non valido.");
            return;
        }

        System.Console.Write("ID hub di destinazione: ");
        if (!Guid.TryParse(System.Console.ReadLine(), out Guid destinationId) || !context.Hubs.ContainsKey(destinationId))
        {
            System.Console.WriteLine("Hub di destinazione non valido.");
            return;
        }

        var result = _analysisService.CheckReachability(context.Network, originId, destinationId);
        if (result.IsFailure)
        {
            System.Console.WriteLine($"{result.Error.Code}: {result.Error.Description}");
            return;
        }

        ReachabilityResult analysis = result.Value;
        System.Console.WriteLine(analysis.CanReach ? "Destinazione raggiungibile." : "Destinazione non raggiungibile.");
        System.Console.WriteLine($"Percorso BFS: {Format(analysis.Path)}");
        System.Console.WriteLine($"Visita BFS: {Format(analysis.BreadthFirstOrder)}");
        System.Console.WriteLine($"Visita DFS: {Format(analysis.DepthFirstOrder)}");

        string Format(IEnumerable<Guid> ids) => string.Join(" -> ", ids.Select(id => context.Hubs[id].Code.Value));
    }
}
