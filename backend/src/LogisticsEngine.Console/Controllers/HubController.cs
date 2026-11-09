using LogisticsEngine.Application.Hubs;
using LogisticsEngine.Application.Hubs.Algorithms;
using LogisticsEngine.Application.Hubs.DeleteHub;
using LogisticsEngine.Application.Hubs.GetAll;
using LogisticsEngine.Application.Hubs.GetBy;
using LogisticsEngine.Application.Hubs.GetById;
using LogisticsEngine.Application.Hubs.RegisterHub;
using LogisticsEngine.Application.Hubs.UpdateHub;
using LogisticsEngine.SharedKernel;
using MediatR;

namespace LogisticsEngine.Console.Controllers;

/// <summary>Gestisce il menu testuale e delega ogni operazione Hub a un request MediatR.</summary>
public sealed class HubController(ISender sender)
{
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        bool running = true;
        while (running)
        {
            System.Console.WriteLine("\n--- Gestione Hub ---\n1. Elenca\n2. Crea\n3. Dettaglio per ID\n4. Modifica\n5. Elimina\n6. Cerca per nome\n7. Ordina\n8. Raggruppa per zona\n9. Collezioni (queue/stack/hashmap/set)\n10. Albero binario\n11. Trie per prefisso\n12. Collega due Hub\n13. Elenca collegamenti\n14. Cammino minimo\n15. BFS\n16. DFS\n17. Cammini semplici (backtracking)\n0. Indietro");
            switch (System.Console.ReadLine()?.Trim())
            {
                case "1": await ListAsync(cancellationToken); break;
                case "2": await CreateAsync(cancellationToken); break;
                case "3": await ShowByIdAsync(cancellationToken); break;
                case "4": await UpdateAsync(cancellationToken); break;
                case "5": await DeleteAsync(cancellationToken); break;
                case "6": await ShowByNameAsync(cancellationToken); break;
                case "7": await SortAsync(cancellationToken); break;
                case "8": await GroupByZoneAsync(cancellationToken); break;
                case "9": await ShowCollectionsAsync(cancellationToken); break;
                case "10": await ShowTreeAsync(cancellationToken); break;
                case "11": await SearchPrefixAsync(cancellationToken); break;
                case "12": await ConnectAsync(cancellationToken); break;
                case "13": await ListConnectionsAsync(cancellationToken); break;
                case "14": await FindShortestPathAsync(cancellationToken); break;
                case "15": await TraverseAsync(HubGraphTraversal.BreadthFirst, cancellationToken); break;
                case "16": await TraverseAsync(HubGraphTraversal.DepthFirst, cancellationToken); break;
                case "17": await FindSimplePathsAsync(cancellationToken); break;
                case "0": running = false; break;
                default: System.Console.WriteLine("Scelta non valida."); break;
            }
        }
    }

    private async Task ListAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<HubDto> hubs = await sender.Send(new GetAllHubsQuery(), cancellationToken);
        if (hubs.Count == 0)
        {
            System.Console.WriteLine("Nessun Hub presente.");
            return;
        }

        foreach (HubDto hub in hubs)
        {
            System.Console.WriteLine($"{hub.Id} | {hub.Name} | {hub.Zone}");
        }
    }

    private async Task CreateAsync(CancellationToken cancellationToken)
    {
        (string? name, string? zone) = ReadHubDetails();
        Result<HubDto> result = await sender.Send(new RegisterHubCommand(name, zone), cancellationToken);
        if (result.IsSuccess)
        {
            System.Console.WriteLine($"Hub creato: {result.Value.Id}");
            return;
        }

        ShowError(result.Error);
    }

    private async Task ShowByIdAsync(CancellationToken cancellationToken)
    {
        Result<HubDto> result = await sender.Send(new GetHubByIdQuery(ReadGuid()), cancellationToken);
        ShowHubOrError(result);
    }

    private async Task ShowByNameAsync(CancellationToken cancellationToken)
    {
        System.Console.Write("Nome Hub: ");
        string? name = System.Console.ReadLine();
        Result<HubDto> result = await sender.Send(new GetHubByNameQuery(name), cancellationToken);
        ShowHubOrError(result);
    }

    private async Task SortAsync(CancellationToken cancellationToken)
    {
        System.Console.Write("Chiave (id, nome, zona): ");
        HubSortProperty property = System.Console.ReadLine()?.Trim().ToLowerInvariant() switch
        {
            "id" => HubSortProperty.Id,
            "nome" => HubSortProperty.Name,
            "zona" => HubSortProperty.Zone,
            _ => HubSortProperty.Id
        };

        IReadOnlyList<HubDto> hubs = await sender.Send(new GetSortedHubsQuery(property), cancellationToken);
        PrintHubs(hubs);
    }

    private async Task GroupByZoneAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<HubGroupDto> groups = await sender.Send(new GetHubsGroupedByZoneQuery(), cancellationToken);
        foreach (HubGroupDto group in groups)
        {
            System.Console.WriteLine($"Zona {group.Zone}:");
            PrintHubs(group.Hubs);
        }
    }

    private async Task ShowCollectionsAsync(CancellationToken cancellationToken)
    {
        HubCollectionsDto collections = await sender.Send(new GetHubCollectionsQuery(), cancellationToken);
        System.Console.WriteLine("Queue, ordine FIFO:");
        PrintHubs(collections.QueueOrder);
        System.Console.WriteLine("Stack, estrazione LIFO:");
        PrintHubs(collections.StackOrder);
        System.Console.WriteLine("HashMap, indicizzata per ID:");
        foreach ((Guid id, HubDto hub) in collections.IdHashMap)
        {
            System.Console.WriteLine($"{id} -> {hub.Name}");
        }

        System.Console.WriteLine($"Set delle zone: {string.Join(", ", collections.Zones.OrderBy(zone => zone, StringComparer.Ordinal))}");
    }

    private async Task ShowTreeAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<HubDto> hubs = await sender.Send(new GetHubTreeOrderQuery(), cancellationToken);
        System.Console.WriteLine("Visita in-order dell'albero binario per ID:");
        PrintHubs(hubs);
    }

    private async Task SearchPrefixAsync(CancellationToken cancellationToken)
    {
        System.Console.Write("Prefisso del nome: ");
        string? prefix = System.Console.ReadLine();
        IReadOnlyList<HubDto> hubs = await sender.Send(new SearchHubsByPrefixQuery(prefix), cancellationToken);
        PrintHubs(hubs);
    }

    private async Task ConnectAsync(CancellationToken cancellationToken)
    {
        Guid firstHubId = ReadGuid("ID primo Hub:");
        Guid secondHubId = ReadGuid("ID secondo Hub:");
        System.Console.Write("Distanza positiva: ");
        if (!decimal.TryParse(System.Console.ReadLine(), out decimal distance))
        {
            System.Console.WriteLine("Distanza non valida.");
            return;
        }

        Result<HubConnectionDto> result = await sender.Send(
            new ConnectHubsCommand(firstHubId, secondHubId, distance), cancellationToken);
        if (result.IsSuccess)
        {
            System.Console.WriteLine($"Collegamento creato: {result.Value.FirstHubId} <-> {result.Value.SecondHubId} ({result.Value.Distance} km)");
            return;
        }

        ShowError(result.Error);
    }

    private async Task ListConnectionsAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<HubConnectionDto> connections = await sender.Send(new GetHubConnectionsQuery(), cancellationToken);
        foreach (HubConnectionDto connection in connections)
        {
            System.Console.WriteLine($"{connection.FirstHubId} <-> {connection.SecondHubId} ({connection.Distance} km)");
        }
    }

    private async Task FindShortestPathAsync(CancellationToken cancellationToken)
    {
        Guid originId = ReadGuid("ID Hub di partenza:");
        Guid destinationId = ReadGuid("ID Hub di destinazione:");
        Result<HubPathDto> result = await sender.Send(new FindShortestHubPathQuery(originId, destinationId), cancellationToken);
        if (result.IsFailure)
        {
            ShowError(result.Error);
            return;
        }

        PrintPath(result.Value);
    }

    private async Task TraverseAsync(HubGraphTraversal traversal, CancellationToken cancellationToken)
    {
        Result<IReadOnlyList<HubDto>> result = await sender.Send(
            new GetHubTraversalQuery(ReadGuid("ID Hub di partenza:"), traversal), cancellationToken);
        if (result.IsFailure)
        {
            ShowError(result.Error);
            return;
        }

        PrintHubs(result.Value);
    }

    private async Task FindSimplePathsAsync(CancellationToken cancellationToken)
    {
        Guid originId = ReadGuid("ID Hub di partenza:");
        Guid destinationId = ReadGuid("ID Hub di destinazione:");
        Result<IReadOnlyList<HubPathDto>> result = await sender.Send(
            new FindSimpleHubPathsQuery(originId, destinationId, _maximumPathsToDisplay), cancellationToken);
        if (result.IsFailure)
        {
            ShowError(result.Error);
            return;
        }

        if (result.Value.Count == 0)
        {
            System.Console.WriteLine("Nessun cammino trovato.");
            return;
        }

        foreach (HubPathDto path in result.Value)
        {
            PrintPath(path);
        }
    }

    private async Task UpdateAsync(CancellationToken cancellationToken)
    {
        Guid id = ReadGuid();
        (string? name, string? zone) = ReadHubDetails();
        Result result = await sender.Send(new UpdateHubCommand(id, name, zone), cancellationToken);
        if (result.IsSuccess)
        {
            System.Console.WriteLine("Hub aggiornato.");
            return;
        }

        ShowError(result.Error);
    }

    private async Task DeleteAsync(CancellationToken cancellationToken)
    {
        Result result = await sender.Send(new DeleteHubCommand(ReadGuid()), cancellationToken);
        if (result.IsSuccess)
        {
            System.Console.WriteLine("Hub eliminato.");
            return;
        }

        ShowError(result.Error);
    }

    private static void ShowHubOrError(Result<HubDto> result)
    {
        if (result.IsSuccess)
        {
            System.Console.WriteLine($"{result.Value.Id} | {result.Value.Name} | {result.Value.Zone}");
            return;
        }

        ShowError(result.Error);
    }

    private static void PrintHubs(IEnumerable<HubDto> hubs)
    {
        foreach (HubDto hub in hubs)
        {
            System.Console.WriteLine($"{hub.Id} | {hub.Name} | {hub.Zone}");
        }
    }

    private static void PrintPath(HubPathDto path)
    {
        System.Console.WriteLine($"{string.Join(" -> ", path.Hubs.Select(hub => hub.Name))} | distanza: {path.TotalDistance} km");
    }

    private static (string? Name, string? Zone) ReadHubDetails()
    {
        System.Console.Write("Nome: ");
        string? name = System.Console.ReadLine();
        System.Console.Write("Zona (2-10 lettere): ");
        string? zone = System.Console.ReadLine();
        return (name, zone);
    }

    private static Guid ReadGuid(string prompt = "ID Hub:")
    {
        System.Console.Write($"{prompt} ");
        return Guid.TryParse(System.Console.ReadLine(), out Guid id) ? id : Guid.Empty;
    }

    private static void ShowError(Error error) => System.Console.WriteLine($"Errore [{error.Code}]: {error.Description}");

    private const int _maximumPathsToDisplay = 10;
}
