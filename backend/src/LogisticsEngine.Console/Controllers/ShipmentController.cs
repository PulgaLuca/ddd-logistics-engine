using LogisticsEngine.Application.Shipments;
using LogisticsEngine.Application.Shipments.CreateShipment;
using LogisticsEngine.Application.Shipments.Delete;
using LogisticsEngine.Application.Shipments.GetAll;
using LogisticsEngine.Application.Shipments.GetById;
using LogisticsEngine.Application.Shipments.UpdateStatus;
using LogisticsEngine.Domain.Shipments.Enums;
using LogisticsEngine.SharedKernel;
using MediatR;

namespace LogisticsEngine.Console.Controllers;

/// <summary>Gestisce l'interazione testuale per i casi d'uso Shipment via MediatR.</summary>
public sealed class ShipmentController(ISender sender)
{
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        bool running = true;
        while (running)
        {
            System.Console.WriteLine("\n--- Gestione spedizioni ---\n1. Elenca\n2. Crea\n3. Dettaglio per ID\n4. Cambia stato\n5. Elimina\n0. Indietro");
            switch (System.Console.ReadLine()?.Trim())
            {
                case "1": await ListAsync(cancellationToken); break;
                case "2": await CreateAsync(cancellationToken); break;
                case "3": await ShowAsync(cancellationToken); break;
                case "4": await ChangeStatusAsync(cancellationToken); break;
                case "5": await DeleteAsync(cancellationToken); break;
                case "0": running = false; break;
                default: System.Console.WriteLine("Scelta non valida."); break;
            }
        }
    }

    private async Task ListAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<ShipmentDto> shipments = await sender.Send(new GetAllShipmentsQuery(), cancellationToken);
        if (shipments.Count == 0)
        {
            System.Console.WriteLine("Nessuna spedizione presente.");
            return;
        }

        foreach (ShipmentDto shipment in shipments)
        {
            Print(shipment);
        }
    }

    private async Task CreateAsync(CancellationToken cancellationToken)
    {
        System.Console.Write("Tracking number: ");
        string? trackingNumber = System.Console.ReadLine();
        Result<ShipmentDto> result = await sender.Send(new CreateShipmentCommand(trackingNumber), cancellationToken);
        if (result.IsSuccess)
        {
            System.Console.WriteLine("Spedizione creata:");
            Print(result.Value);
            return;
        }

        ShowError(result.Error);
    }

    private async Task ShowAsync(CancellationToken cancellationToken)
    {
        Result<ShipmentDto> result = await sender.Send(new GetShipmentByIdQuery(ReadGuid()), cancellationToken);
        if (result.IsSuccess)
        {
            Print(result.Value);
            return;
        }

        ShowError(result.Error);
    }

    private async Task ChangeStatusAsync(CancellationToken cancellationToken)
    {
        Guid id = ReadGuid();
        System.Console.Write("Nuovo stato (InTransit, Delivered, Cancelled): ");
        string? value = System.Console.ReadLine();
        if (!Enum.TryParse(value, ignoreCase: true, out ShipmentStatus status))
        {
            System.Console.WriteLine("Stato non valido.");
            return;
        }

        Result result = await sender.Send(new UpdateShipmentStatusCommand(id, status), cancellationToken);
        ShowResult(result, "Stato aggiornato.");
    }

    private async Task DeleteAsync(CancellationToken cancellationToken)
    {
        Result result = await sender.Send(new DeleteShipmentCommand(ReadGuid()), cancellationToken);
        ShowResult(result, "Spedizione eliminata.");
    }

    private static void Print(ShipmentDto shipment) =>
        System.Console.WriteLine($"{shipment.Id} | {shipment.TrackingNumber} | {shipment.Status}");

    private static Guid ReadGuid()
    {
        System.Console.Write("ID spedizione: ");
        return Guid.TryParse(System.Console.ReadLine(), out Guid id) ? id : Guid.Empty;
    }

    private static void ShowResult(Result result, string successMessage)
    {
        if (result.IsSuccess)
        {
            System.Console.WriteLine(successMessage);
            return;
        }

        ShowError(result.Error);
    }

    private static void ShowError(Error error) => System.Console.WriteLine($"Errore [{error.Code}]: {error.Description}");
}
