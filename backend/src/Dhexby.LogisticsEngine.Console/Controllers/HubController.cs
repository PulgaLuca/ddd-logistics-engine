using Dhexby.LogisticsEngine.Application.Hubs;
using Dhexby.LogisticsEngine.Application.Hubs.GetHubById;
using Dhexby.LogisticsEngine.Application.Hubs.RegisterHub;
using Dhexby.SharedKernel;
using MediatR;

namespace Dhexby.LogisticsEngine.Console.Controllers;

/// <summary>Adapter console per i casi d'uso relativi agli hub.</summary>
public sealed class HubController
{
    private readonly IMediator _sender;

    public HubController(IMediator sender)
    {
        _sender = sender;
    }

    public async Task RegisterAsync(CancellationToken cancellationToken = default)
    {
        System.Console.Write("Codice hub (es. HUB-0001): ");
        string code = System.Console.ReadLine()?.Trim() ?? string.Empty;

        System.Console.Write("Latitudine: ");
        if (!double.TryParse(System.Console.ReadLine(), System.Globalization.CultureInfo.InvariantCulture, out double latitude))
        {
            System.Console.WriteLine("Latitudine non valida.");
            return;
        }

        System.Console.Write("Longitudine: ");
        if (!double.TryParse(System.Console.ReadLine(), System.Globalization.CultureInfo.InvariantCulture, out double longitude))
        {
            System.Console.WriteLine("Longitudine non valida.");
            return;
        }

        Result<Guid> result = await _sender.Send(new RegisterHubCommand(code, latitude, longitude), cancellationToken);

        if (result.IsSuccess)
        {
            System.Console.WriteLine($"Hub registrato. ID: {result.Value}");
            return;
        }

        PrintError(result.Error);
    }

    public async Task GetByIdAsync(CancellationToken cancellationToken = default)
    {
        System.Console.Write("ID hub: ");
        if (!Guid.TryParse(System.Console.ReadLine(), out Guid id))
        {
            System.Console.WriteLine("ID non valido.");
            return;
        }

        Result<HubResponse> result = await _sender.Send(new GetHubByIdQuery(id), cancellationToken);

        if (result.IsSuccess)
        {
            HubResponse hub = result.Value;
            System.Console.WriteLine($"{hub.Code} | {hub.Status} | {hub.Latitude}, {hub.Longitude} | {hub.Id}");
            return;
        }

        PrintError(result.Error);
    }

    private static void PrintError(Error error)
    {
        System.Console.WriteLine($"{error.Code}: {error.Description}");
    }
}
