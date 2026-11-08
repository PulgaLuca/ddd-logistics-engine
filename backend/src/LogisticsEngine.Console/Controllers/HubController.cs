using LogisticsEngine.Application.Hubs;
using LogisticsEngine.Application.Shared;
using LogisticsEngine.Application.Hubs.DeleteHub;
using LogisticsEngine.Application.Hubs.GetAll;
using LogisticsEngine.Application.Hubs.GetById;
using LogisticsEngine.Application.Hubs.RegisterHub;
using LogisticsEngine.Application.Hubs.UpdateHub;
using LogisticsEngine.SharedKernel;

namespace LogisticsEngine.Console.Controllers;

/// <summary>Presenta il menu Hub e mantiene l'interazione testuale fuori dai livelli di dominio.</summary>
public sealed class HubController(
    ICommandHandler<GetAllCommand, IReadOnlyList<HubDto>> getAll,
    ICommandHandler<RegisterHubCommand, Result<HubDto>> registerHub,
    ICommandHandler<GetByIdCommand, Result<HubDto>> getById,
    ICommandHandler<UpdateHubCommand, Result> updateHub,
    ICommandHandler<DeleteHubCommand, Result> deleteHub)
{
    public void Run()
    {
        bool running = true;
        while (running)
        {
            System.Console.WriteLine("\n--- Gestione Hub ---\n1. Elenca\n2. Crea\n3. Dettaglio\n4. Modifica\n5. Elimina\n6. Ordina per ID\n7. Raggruppa per zona\n0. Esci");
            switch (System.Console.ReadLine()?.Trim())
            {
                case "1": List(); break;
                case "2": Create(); break;
                case "3": Show(); break;
                case "4": Update(); break;
                case "5": Delete(); break;
                case "0": running = false; break;
                default: System.Console.WriteLine("Scelta non valida."); break;
            }
        }
    }

    private void List()
    {
        IReadOnlyList<HubDto> hubs = getAll.Handle(new GetAllCommand());
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

    private void Create()
    {
        (string? name, string? zone) = ReadHubDetails();
        Result<HubDto> result = registerHub.Handle(new RegisterHubCommand(name, zone));
        if (result.IsSuccess)
        {
            System.Console.WriteLine($"Hub creato: {result.Value.Id}");
        }
        else
        {
            ShowError(result.Error);
        }
    }

    private void Show()
    {
        Result<HubDto> result = getById.Handle(new GetByIdCommand(ReadGuid()));
        if (result.IsSuccess)
        {
            System.Console.WriteLine($"{result.Value.Id} | {result.Value.Name} | {result.Value.Zone}");
        }
        else
        {
            ShowError(result.Error);
        }
    }

    private void Update()
    {
        Guid id = ReadGuid();
        (string? name, string? zone) = ReadHubDetails();
        Result result = updateHub.Handle(new UpdateHubCommand(id, name, zone));
        if (result.IsSuccess)
        {
            System.Console.WriteLine("Hub aggiornato.");
        }
        else
        {
            ShowError(result.Error);
        }
    }

    private void Delete()
    {
        Result result = deleteHub.Handle(new DeleteHubCommand(ReadGuid()));
        if (result.IsSuccess)
        {
            System.Console.WriteLine("Hub eliminato.");
        }
        else
        {
            ShowError(result.Error);
        }
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
