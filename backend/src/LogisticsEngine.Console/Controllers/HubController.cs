using LogisticsEngine.Application.Hubs;
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
            System.Console.WriteLine("\n--- Gestione Hub ---\n1. Elenca\n2. Crea\n3. Dettaglio per ID\n4. Modifica\n5. Elimina\n6. Cerca per nome\n0. Esci");
            switch (System.Console.ReadLine()?.Trim())
            {
                case "1": await ListAsync(cancellationToken); break;
                case "2": await CreateAsync(cancellationToken); break;
                case "3": await ShowByIdAsync(cancellationToken); break;
                case "4": await UpdateAsync(cancellationToken); break;
                case "5": await DeleteAsync(cancellationToken); break;
                case "6": await ShowByNameAsync(cancellationToken); break;
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

    private static (string? Name, string? Zone) ReadHubDetails()
    {
        System.Console.Write("Nome: ");
        string? name = System.Console.ReadLine();
        System.Console.Write("Zona (2-10 lettere): ");
        string? zone = System.Console.ReadLine();
        return (name, zone);
    }

    private static Guid ReadGuid()
    {
        System.Console.Write("ID Hub: ");
        return Guid.TryParse(System.Console.ReadLine(), out Guid id) ? id : Guid.Empty;
    }

    private static void ShowError(Error error) => System.Console.WriteLine($"Errore [{error.Code}]: {error.Description}");
}
