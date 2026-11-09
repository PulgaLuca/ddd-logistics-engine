using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace LogisticsEngine.IntegrationTests;

/// <summary>Verifica flussi HTTP completi usando DI e repository JSON reali in un archivio isolato.</summary>
public sealed class ApiWorkflowTests : IDisposable
{
    private readonly string _dataDirectory = Path.Combine(Path.GetTempPath(), $"logistics-tests-{Guid.NewGuid():N}");
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ApiWorkflowTests()
    {
        // 1. Avvia l'app in ambiente Testing e indirizza la persistenza a una cartella isolata.
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureAppConfiguration((_, configuration) =>
                configuration.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Storage:DataDirectory"] = _dataDirectory
                }));
        });

        // 2. Il client invia richieste HTTP al server in-process senza aprire una porta di rete.
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Hub_http_workflow_creates_and_reads_persisted_resource()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        // 3. POST attraversa routing, model binding, controller, MediatR, handler e repository JSON.
        using HttpResponseMessage create = await _client.PostAsJsonAsync(
            "/api/v1/hubs", new { name = "Milano", zone = "NORD" }, cancellationToken);
        create.StatusCode.ShouldBe(HttpStatusCode.Created);
        create.Headers.Location.ShouldNotBeNull();

        // 4. Estrae dal DTO l'ID creato, come farebbe un client HTTP reale.
        using JsonDocument created = JsonDocument.Parse(await create.Content.ReadAsStringAsync(cancellationToken));
        string id = created.RootElement.GetProperty("id").GetString()!;

        // 5. GET ripercorre l'intera pipeline e dimostra che il repository ha salvato il dato.
        using HttpResponseMessage read = await _client.GetAsync($"/api/v1/hubs/{id}", cancellationToken);
        read.StatusCode.ShouldBe(HttpStatusCode.OK);
        using JsonDocument hub = JsonDocument.Parse(await read.Content.ReadAsStringAsync(cancellationToken));
        hub.RootElement.GetProperty("name").GetString().ShouldBe("Milano");
        hub.RootElement.GetProperty("zone").GetString().ShouldBe("NORD");
    }

    [Fact]
    public async Task Invalid_hub_request_returns_client_error_problem()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        // Il caso percorre validazione FluentValidation e conversione dell'errore in ProblemDetails HTTP.
        using HttpResponseMessage response = await _client.PostAsJsonAsync(
            "/api/v1/hubs", new { name = " ", zone = "NORD" }, cancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/problem+json");
    }

    [Fact]
    public async Task Shipment_http_workflow_creates_and_lists_resource()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        // Verifica un secondo caso d'uso pubblico con gli stessi layer e lo stesso storage isolato.
        using HttpResponseMessage create = await _client.PostAsJsonAsync(
            "/api/v1/shipments", new { trackingNumber = "ABCD1234" }, cancellationToken);
        create.StatusCode.ShouldBe(HttpStatusCode.Created);

        using HttpResponseMessage list = await _client.GetAsync("/api/v1/shipments", cancellationToken);
        list.StatusCode.ShouldBe(HttpStatusCode.OK);
        using JsonDocument shipments = JsonDocument.Parse(await list.Content.ReadAsStringAsync(cancellationToken));
        shipments.RootElement.GetArrayLength().ShouldBe(1);
        shipments.RootElement[0].GetProperty("trackingNumber").GetString().ShouldBe("ABCD1234");
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();

        // Rimuove esclusivamente la directory temporanea creata da questa istanza di test.
        if (Directory.Exists(_dataDirectory))
        {
            Directory.Delete(_dataDirectory, recursive: true);
        }
    }
}
