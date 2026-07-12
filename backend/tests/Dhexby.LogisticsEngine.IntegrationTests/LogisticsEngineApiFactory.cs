using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Dhexby.LogisticsEngine.IntegrationTests;

/// <summary>
/// Host di test: identico all'app reale, ma con lo store JSON rediretto
/// su un file temporaneo isolato, ripulito a fine esecuzione.
/// </summary>
public sealed class LogisticsEngineApiFactory : WebApplicationFactory<Program>
{
    private readonly string _storagePath = Path.Combine(
        Path.GetTempPath(),
        $"logistics-engine-tests-{Guid.NewGuid():N}.json");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configuration) =>
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Storage:ShipmentsFilePath"] = _storagePath,
            }));
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing && File.Exists(_storagePath))
        {
            File.Delete(_storagePath);
        }
    }
}
