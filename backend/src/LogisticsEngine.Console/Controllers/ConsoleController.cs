namespace LogisticsEngine.Console.Controllers;

/// <summary>Seleziona l'area funzionale della Console.</summary>
public sealed class ConsoleController(HubController hubController, ShipmentController shipmentController)
{
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        bool running = true;
        while (running)
        {
            System.Console.WriteLine("\n--- LogisticsEngine ---\n1. Gestione Hub\n2. Gestione spedizioni\n0. Esci");
            switch (System.Console.ReadLine()?.Trim())
            {
                case "1": await hubController.RunAsync(cancellationToken); break;
                case "2": await shipmentController.RunAsync(cancellationToken); break;
                case "0": running = false; break;
                default: System.Console.WriteLine("Scelta non valida."); break;
            }
        }
    }
}
