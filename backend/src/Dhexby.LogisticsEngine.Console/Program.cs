using Dhexby.LogisticsEngine.Application;
using Dhexby.LogisticsEngine.Console.Controllers;
using Dhexby.LogisticsEngine.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory
});

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);
builder.Services.AddTransient<HubController>();
builder.Services.AddTransient<DispatchHistoryController>();
builder.Services.AddTransient<NetworkAnalysisController>();
builder.Services.AddTransient<OrderQueueController>();
builder.Services.AddTransient<ShipmentPricingController>();

using IHost host = builder.Build();
await host.StartAsync();

try
{
    await RunConsoleAsync(host.Services);
}
finally
{
    await host.StopAsync();
}

static async Task RunConsoleAsync(IServiceProvider services)
{
    bool running = true;

    while (running)
    {
        Console.WriteLine();
        Console.WriteLine("Dhexby Logistics Engine");
        Console.WriteLine("1. Registra hub");
        Console.WriteLine("2. Cerca hub per ID");
        Console.WriteLine("3. Demo cronologia dispatch (Stack undo/redo)");
        Console.WriteLine("4. Demo analisi rete (BFS/DFS e lookup con Dictionary)");
        Console.WriteLine("5. Demo code ordini (FIFO e priorità)");
        Console.WriteLine("6. Calcola prezzo spedizione");
        Console.WriteLine("0. Esci");
        Console.Write("Scelta: ");

        string? choice = Console.ReadLine()?.Trim();
        await using AsyncServiceScope scope = services.CreateAsyncScope();

        switch (choice)
        {
            case "1":
                await scope.ServiceProvider.GetRequiredService<HubController>().RegisterAsync();
                break;
            case "2":
                await scope.ServiceProvider.GetRequiredService<HubController>().GetByIdAsync();
                break;
            case "3":
                scope.ServiceProvider.GetRequiredService<DispatchHistoryController>().ShowDemo();
                break;
            case "4":
                await scope.ServiceProvider.GetRequiredService<NetworkAnalysisController>().ShowDemoAsync();
                break;
            case "5":
                scope.ServiceProvider.GetRequiredService<OrderQueueController>().ShowDemo();
                break;
            case "6":
                scope.ServiceProvider.GetRequiredService<ShipmentPricingController>().Calculate();
                break;
            case "0":
                running = false;
                break;
            default:
                Console.WriteLine("Scelta non valida.");
                break;
        }
    }
}
